#!/usr/bin/env node
/**
 * Generates device-metrics.json from Appium/Wdio test output.
 * Prefers JUnit XML (structured) when available; falls back to spec reporter output.
 * Run after wdio: node scripts/device-metrics.js [wdio-output.log] [junit-dir]
 */

const fs = require('fs');
const path = require('path');

const REPORTS_DIR = path.join(__dirname, '..', 'reports');
const DEVICE_METRICS_PATH = path.join(REPORTS_DIR, 'device-metrics.json');
const APPIUM_REPORTS = path.join(__dirname, '..', 'automation', 'mobile', 'appium', 'reports');

function parseJunitXml(content) {
  const tests = parseInt(content.match(/tests="(\d+)"/i)?.[1], 10) || 0;
  const failures = parseInt(content.match(/failures="(\d+)"/i)?.[1], 10) || 0;
  const errors = parseInt(content.match(/errors="(\d+)"/i)?.[1], 10) || 0;
  const failed = failures + errors;
  const passed = Math.max(0, tests - failed);
  return { passed, failed, total: tests || passed + failed };
}

function parseSpecOutput(content) {
  let passed = 0, failed = 0;
  const passingMatch = content.match(/(\d+)\s+passing/);
  const failingMatch = content.match(/(\d+)\s+failing/);
  if (passingMatch) passed = parseInt(passingMatch[1], 10);
  if (failingMatch) failed = parseInt(failingMatch[1], 10);
  return { passed, failed, total: passed + failed };
}

function collectFromJunit(junitDir) {
  const dir = path.isAbsolute(junitDir) ? junitDir : path.join(process.cwd(), junitDir);
  if (!fs.existsSync(dir)) return null;
  let smoke = { passed: 0, failed: 0 }, fullFlow = { passed: 0, failed: 0 }, total = { passed: 0, failed: 0 };
  try {
    const files = fs.readdirSync(dir).filter((f) => f.endsWith('.xml')) || [];
    for (const f of files) {
      const content = fs.readFileSync(path.join(dir, f), 'utf8');
      const parsed = parseJunitXml(content);
      total.passed += parsed.passed;
      total.failed += parsed.failed;
      const name = f.toLowerCase();
      if (name.includes('smoke')) {
        smoke.passed += parsed.passed;
        smoke.failed += parsed.failed;
      } else if (name.includes('fullflow') || name.includes('full-flow')) {
        fullFlow.passed += parsed.passed;
        fullFlow.failed += parsed.failed;
      } else if (smoke.passed === 0 && smoke.failed === 0) {
        smoke = { passed: parsed.passed, failed: parsed.failed };
      } else {
        fullFlow = { passed: parsed.passed, failed: parsed.failed };
      }
    }
    if (total.passed > 0 || total.failed > 0) {
      return { passed: total.passed, failed: total.failed, total: total.passed + total.failed, smoke, fullFlow };
    }
  } catch { /* ignore */ }
  return null;
}

function collectFromLogs() {
  const metrics = {
    timestamp: new Date().toISOString(),
    device: { passed: 0, failed: 0, total: 0, smoke: { passed: 0, failed: 0 }, fullFlow: { passed: 0, failed: 0 } },
  };

  const args = process.argv.slice(2);
  const logPaths = args.filter((a) => !a.includes('junit'));
  const junitArg = args.find((a) => a.includes('junit'));

  // Prefer JUnit XML (structured) when available
  const junitDirs = junitArg ? [junitArg] : [
    path.join(process.cwd(), 'reports', 'junit'),
    path.join(APPIUM_REPORTS, 'junit'),
  ];
  for (const d of junitDirs) {
    const fromJunit = collectFromJunit(d);
    if (fromJunit && fromJunit.total > 0) {
      return {
        timestamp: new Date().toISOString(),
        device: {
          passed: fromJunit.passed,
          failed: fromJunit.failed,
          total: fromJunit.total,
          smoke: fromJunit.smoke || { passed: 0, failed: 0 },
          fullFlow: fromJunit.fullFlow || { passed: 0, failed: 0 },
        },
      };
    }
  }

  const defaultLogPaths = logPaths.length > 0 ? logPaths : [
    path.join(APPIUM_REPORTS, 'wdio.log'),
    path.join(REPORTS_DIR, 'wdio.log'),
  ];

  for (const logPath of defaultLogPaths) {
    const fullPath = path.isAbsolute(logPath) ? logPath : path.join(process.cwd(), logPath);
    if (!fs.existsSync(fullPath)) continue;
    try {
      const content = fs.readFileSync(fullPath, 'utf8');
      const parsed = parseSpecOutput(content);
      metrics.device.passed += parsed.passed;
      metrics.device.failed += parsed.failed;
      metrics.device.total += parsed.passed + parsed.failed;
      const name = path.basename(fullPath).toLowerCase();
      if (name.includes('smoke')) {
        metrics.device.smoke.passed += parsed.passed;
        metrics.device.smoke.failed += parsed.failed;
      } else if (name.includes('fullflow') || name.includes('full-flow')) {
        metrics.device.fullFlow.passed += parsed.passed;
        metrics.device.fullFlow.failed += parsed.failed;
      }
    } catch { /* ignore */ }
  }

  if (metrics.device.total === 0 && defaultLogPaths.length === 0) {
    if (fs.existsSync(APPIUM_REPORTS)) {
      const files = fs.readdirSync(APPIUM_REPORTS) || [];
      for (const f of files) {
        if (f.endsWith('.log') || f.endsWith('.txt')) {
          const full = path.join(APPIUM_REPORTS, f);
          const content = fs.readFileSync(full, 'utf8').catch(() => '');
          const parsed = parseSpecOutput(content);
          if (parsed.total > 0) {
            metrics.device.passed += parsed.passed;
            metrics.device.failed += parsed.failed;
            metrics.device.total += parsed.total;
            break;
          }
        }
      }
    }
  }

  return metrics;
}

const metrics = collectFromLogs();
fs.mkdirSync(REPORTS_DIR, { recursive: true });
fs.writeFileSync(DEVICE_METRICS_PATH, JSON.stringify(metrics, null, 2));
console.log('Device metrics written to', DEVICE_METRICS_PATH);
