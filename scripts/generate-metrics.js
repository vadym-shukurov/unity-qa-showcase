#!/usr/bin/env node
/**
 * Generates metrics JSON and HTML dashboard from test results.
 * Run after tests: node scripts/generate-metrics.js
 * Input: reports/*.xml (JUnit/NUnit XML)
 * Output: reports/metrics.json, reports/dashboard.html
 */

const fs = require('fs');
const path = require('path');

const REPORTS_DIR = path.join(__dirname, '..', 'reports');
const OUTPUT_JSON = path.join(REPORTS_DIR, 'metrics.json');
const OUTPUT_HTML = path.join(REPORTS_DIR, 'dashboard.html');

function parseXmlFile(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8');
    // Unity/NUnit XML: result="Passed" or outcome="Passed"
    const passed = (content.match(/(?:result|outcome)="Passed"/gi) || []).length;
    const failed = (content.match(/(?:result|outcome)="Failed"/gi) || []).length;
    const skipped = (content.match(/(?:result|outcome)="Skipped"/gi) || []).length;
    const inconclusive = (content.match(/(?:result|outcome)="Inconclusive"/gi) || []).length;
    const total = passed + failed + skipped + inconclusive;
    const durationMatch = content.match(/(?:duration|time)="([\d.]+)"/);
    const duration = durationMatch ? parseFloat(durationMatch[1]) : 0;
    return { passed, failed, skipped, inconclusive, total, duration };
  } catch {
    return null;
  }
}

/** Extract smoke-specific results from NUnit XML (test-case elements with SmokeFlowPlayModeTests). */
function parseSmokeFromXml(filePath) {
  try {
    const content = fs.readFileSync(filePath, 'utf8');
    let smokePassed = 0, smokeFailed = 0;
    const cases = content.split(/<test-case\s/);
    for (const block of cases) {
      if (!block.includes('SmokeFlowPlayModeTests')) continue;
      const resultMatch = block.match(/(?:result|outcome)="(Passed|Failed|Skipped|Inconclusive)"/i);
      const result = (resultMatch ? resultMatch[1] : '').toLowerCase();
      if (result === 'passed') smokePassed++;
      else if (result === 'failed') smokeFailed++;
    }
    return { smokePassed, smokeFailed, smokeTotal: smokePassed + smokeFailed };
  } catch {
    return { smokePassed: 0, smokeFailed: 0, smokeTotal: 0 };
  }
}

function collectMetrics() {
  const metrics = {
    timestamp: new Date().toISOString(),
    editMode: { passed: 0, failed: 0, skipped: 0, total: 0, duration: 0 },
    playMode: { passed: 0, failed: 0, skipped: 0, total: 0, duration: 0 },
    smoke: { passed: 0, failed: 0, total: 0 },
    smokePassRate: 0,
    flakyRate: 0,
  };

  if (!fs.existsSync(REPORTS_DIR)) {
    fs.mkdirSync(REPORTS_DIR, { recursive: true });
    return metrics;
  }

  function findXmlFiles(dir) {
    const files = [];
    const entries = fs.readdirSync(dir, { withFileTypes: true }) || [];
    for (const e of entries) {
      const full = path.join(dir, e.name);
      if (e.isDirectory()) files.push(...findXmlFiles(full));
      else if (e.name.endsWith('.xml')) files.push(full);
    }
    return files;
  }
  const files = findXmlFiles(REPORTS_DIR);
  for (const fullPath of files) {
    const data = parseXmlFile(fullPath);
    if (!data) continue;
    const pathLower = fullPath.toLowerCase().replace(/\\/g, '/');
    const target = pathLower.includes('editmode') || pathLower.includes('/edit/') ? metrics.editMode : metrics.playMode;
    target.passed += data.passed;
    target.failed += data.failed;
    target.skipped += data.skipped;
    target.total += data.total;
    target.duration += data.duration;
    // Smoke tests are in Play Mode; extract smoke-specific counts
    if (pathLower.includes('playmode') || pathLower.includes('/play/')) {
      const smoke = parseSmokeFromXml(fullPath);
      metrics.smoke.passed += smoke.smokePassed;
      metrics.smoke.failed += smoke.smokeFailed;
      metrics.smoke.total += smoke.smokeTotal;
    }
  }

  const totalPassed = metrics.editMode.passed + metrics.playMode.passed;
  const totalFailed = metrics.editMode.failed + metrics.playMode.failed;
  const total = metrics.editMode.total + metrics.playMode.total;
  // Smoke pass rate: smoke-specific (release gate); fallback to overall if no smoke XML
  metrics.smokePassRate = metrics.smoke.total > 0
    ? (metrics.smoke.passed / metrics.smoke.total * 100).toFixed(1)
    : (total > 0 ? (totalPassed / total * 100).toFixed(1) : 0);
  metrics.flakyRate = total > 0 ? (metrics.playMode.failed / total * 100).toFixed(1) : 0;

  // Merge device metrics when present (from device farm workflow)
  const deviceMetricsPath = path.join(REPORTS_DIR, 'device-metrics.json');
  if (fs.existsSync(deviceMetricsPath)) {
    try {
      metrics.device = JSON.parse(fs.readFileSync(deviceMetricsPath, 'utf8')).device;
    } catch { /* ignore */ }
  }

  // Merge telemetry from TestTelemetry (reports/telemetry.json or artifact paths)
  const telemetryPaths = [
    path.join(REPORTS_DIR, 'telemetry.json'),
    path.join(REPORTS_DIR, 'playmode', 'reports', 'telemetry.json'),
    path.join(REPORTS_DIR, 'playmode', 'telemetry.json'),
  ];
  for (const p of telemetryPaths) {
    if (fs.existsSync(p)) {
      try {
        metrics.telemetry = JSON.parse(fs.readFileSync(p, 'utf8'));
        break;
      } catch { /* ignore */ }
    }
  }

  return metrics;
}

function escapeHtml(str) {
  if (str == null) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#39;');
}

function generateHtml(metrics, history) {
  const runs = (history && history.runs) || [];
  const trendRows = runs.slice(0, 10).map(r =>
    `<tr><td>${escapeHtml(new Date(r.timestamp).toLocaleString())}</td><td>${escapeHtml(r.smokePassRate)}%</td></tr>`
  ).join('');
  return `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>QA Metrics Dashboard</title>
  <style>
    * { box-sizing: border-box; }
    body { font-family: system-ui, sans-serif; margin: 2rem; background: #1a1a2e; color: #eee; }
    h1 { color: #0f3460; }
    .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: 1rem; margin: 2rem 0; }
    .card { background: #16213e; padding: 1.5rem; border-radius: 8px; }
    .card h3 { margin: 0 0 0.5rem; font-size: 0.9rem; color: #aaa; }
    .card .value { font-size: 2rem; font-weight: bold; }
    .pass { color: #4ade80; }
    .fail { color: #f87171; }
    .meta { color: #888; font-size: 0.85rem; margin-top: 2rem; }
  </style>
</head>
<body>
  <h1>QA Metrics Dashboard</h1>
  <p>Unity Mobile QA Automation Showcase</p>
  <div class="grid">
    <div class="card">
      <h3>Edit Mode</h3>
      <div class="value pass">${escapeHtml(metrics.editMode.passed)} passed</div>
      <div class="value fail">${escapeHtml(metrics.editMode.failed)} failed</div>
      <div>${escapeHtml(metrics.editMode.total)} total · ${escapeHtml(metrics.editMode.duration.toFixed(1))}s</div>
    </div>
    <div class="card">
      <h3>Play Mode</h3>
      <div class="value pass">${escapeHtml(metrics.playMode.passed)} passed</div>
      <div class="value fail">${escapeHtml(metrics.playMode.failed)} failed</div>
      <div>${escapeHtml(metrics.playMode.total)} total · ${escapeHtml(metrics.playMode.duration.toFixed(1))}s</div>
    </div>
    <div class="card">
      <h3>Smoke (Release Gate)</h3>
      <div class="value ${(metrics.smoke && metrics.smoke.failed === 0) ? 'pass' : 'fail'}">${escapeHtml(metrics.smokePassRate)}%</div>
      <div>${escapeHtml(metrics.smoke?.passed ?? '-')} passed · ${escapeHtml(metrics.smoke?.failed ?? '-')} failed</div>
    </div>
    ${metrics.device ? `
    <div class="card">
      <h3>Device (Appium)</h3>
      <div class="value pass">${escapeHtml(metrics.device.passed)} passed</div>
      <div class="value fail">${escapeHtml(metrics.device.failed)} failed</div>
      <div>${escapeHtml(metrics.device.total)} total</div>
    </div>
    ` : ''}
    <div class="card">
      <h3>Last Updated</h3>
      <div class="value">${escapeHtml(new Date(metrics.timestamp).toLocaleString())}</div>
    </div>
  </div>
  ${(metrics.telemetry && metrics.telemetry.events && metrics.telemetry.events.length) ? `
  <h2>Performance Telemetry</h2>
  <table style="width:100%; border-collapse: collapse; margin-top: 1rem;">
    <thead><tr><th style="text-align:left;">Metric</th><th>Value</th><th>Unit</th></tr></thead>
    <tbody>${metrics.telemetry.events.slice(0, 20).map(e =>
      `<tr><td>${escapeHtml(e.name)}</td><td>${escapeHtml(Number(e.value).toFixed(2))}</td><td>${escapeHtml(e.unit || '-')}</td></tr>`
    ).join('')}</tbody>
  </table>
  ` : ''}
  <h2>Historical Trend (Last 10 Runs)</h2>
  <table style="width:100%; border-collapse: collapse; margin-top: 1rem;">
    <thead><tr><th style="text-align:left;">Timestamp</th><th>Smoke Pass Rate</th></tr></thead>
    <tbody>${trendRows || '<tr><td colspan="2">No history yet</td></tr>'}</tbody>
  </table>
  <p class="meta">Generated by scripts/generate-metrics.js. History: metrics-history.json (max ${escapeHtml(MAX_HISTORY)} runs).</p>
</body>
</html>`;
}

const HISTORY_FILE = path.join(REPORTS_DIR, 'metrics-history.json');
const MAX_HISTORY = 50;

function loadHistory() {
  try {
    const data = fs.readFileSync(HISTORY_FILE, 'utf8');
    return JSON.parse(data);
  } catch {
    return { runs: [] };
  }
}

function saveHistory(metrics) {
  const history = loadHistory();
  history.runs.unshift({
    timestamp: metrics.timestamp,
    smokePassRate: metrics.smokePassRate,
    smoke: metrics.smoke ? { ...metrics.smoke } : undefined,
    editMode: { ...metrics.editMode },
    playMode: { ...metrics.playMode },
  });
  history.runs = history.runs.slice(0, MAX_HISTORY);
  fs.writeFileSync(HISTORY_FILE, JSON.stringify(history, null, 2));
}

const metrics = collectMetrics();
fs.mkdirSync(REPORTS_DIR, { recursive: true });
fs.writeFileSync(OUTPUT_JSON, JSON.stringify(metrics, null, 2));
saveHistory(metrics);
fs.writeFileSync(OUTPUT_HTML, generateHtml(metrics, loadHistory()));
console.log('Metrics written to', OUTPUT_JSON, OUTPUT_HTML, 'and', HISTORY_FILE);
