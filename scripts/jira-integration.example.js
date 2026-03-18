/**
 * Example: Create Jira issues from failed tests.
 * Configure JIRA_URL, JIRA_TOKEN, JIRA_PROJECT. Run: node scripts/jira-integration.example.js
 *
 * In production: run as CI step when tests fail. Links failures to release.
 */

const fs = require('fs');
const path = require('path');

const REPORTS_DIR = path.join(__dirname, '..', 'reports');
const JIRA_URL = process.env.JIRA_URL || 'https://your-domain.atlassian.net';
const JIRA_PROJECT = process.env.JIRA_PROJECT || 'QA';

function isValidJiraUrl(url) {
  try {
    const u = new URL(url);
    const allowed = ['atlassian.net', 'atlassian.com'];
    return u.protocol === 'https:' && allowed.some(h => u.hostname.endsWith(h));
  } catch {
    return false;
  }
}

function extractFailedTests() {
  const failed = [];
  if (!fs.existsSync(REPORTS_DIR)) return failed;
  const files = fs.readdirSync(REPORTS_DIR).filter(f => f.endsWith('.xml'));
  for (const f of files) {
    const content = fs.readFileSync(path.join(REPORTS_DIR, f), 'utf8');
    const matches = content.matchAll(/<test-case[^>]*fullname="([^"]*)"[^>]*result="Failed"[^>]*>/g);
    for (const m of matches) failed.push({ name: m[1], file: f });
  }
  return failed;
}

function createJiraIssue(test) {
  return {
    fields: {
      project: { key: JIRA_PROJECT },
      summary: `[Automated] Test failed: ${test.name}`,
      description: `Failed test: ${test.name}\nSource: ${test.file}`,
      issuetype: { name: 'Bug' },
      labels: ['automated', 'regression'],
    },
  };
}

const failed = extractFailedTests();
console.log(`Found ${failed.length} failed tests.`);
if (failed.length > 0 && process.env.JIRA_TOKEN) {
  if (!isValidJiraUrl(JIRA_URL)) {
    console.error('Invalid JIRA_URL (must be HTTPS and atlassian.net). SSRF prevention.');
    process.exit(1);
  }
  console.log('Would create Jira issues:', failed.map(f => f.name));
  // fetch(JIRA_URL + '/rest/api/3/issue', { method: 'POST', body: JSON.stringify(createJiraIssue(f)) })
} else if (failed.length > 0) {
  console.log('Set JIRA_TOKEN to create issues. Example payload:', JSON.stringify(createJiraIssue(failed[0]), null, 2));
}
