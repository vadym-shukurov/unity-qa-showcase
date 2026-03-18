#!/usr/bin/env node
/**
 * Example: Report test failures to JIRA/Slack.
 * Run after CI failure: node scripts/report-failure.example.js
 * Requires: JIRA_URL, JIRA_TOKEN (or SLACK_WEBHOOK_URL) in env.
 *
 * Usage:
 *   FAILED_XML=reports/playmode/results.xml node scripts/report-failure.example.js
 *
 * SSRF prevention: JIRA_URL and SLACK_WEBHOOK_URL validated (HTTPS + allowlist).
 */
const fs = require('fs');
const path = require('path');

const xmlPath = process.env.FAILED_XML || 'TestResults/playmode-results.xml';
const jiraUrl = process.env.JIRA_URL;
const jiraToken = process.env.JIRA_TOKEN;
const slackWebhook = process.env.SLACK_WEBHOOK_URL;

function isValidJiraUrl(url) {
  try {
    const u = new URL(url);
    const allowed = ['atlassian.net', 'atlassian.com'];
    return u.protocol === 'https:' && allowed.some(h => u.hostname.endsWith(h));
  } catch {
    return false;
  }
}

function isValidSlackWebhookUrl(url) {
  try {
    const u = new URL(url);
    const allowed = ['slack.com', 'hooks.slack.com'];
    return u.protocol === 'https:' && allowed.some(h => u.hostname.endsWith(h));
  } catch {
    return false;
  }
}

function parseFailures(xmlPath) {
  if (!fs.existsSync(xmlPath)) return [];
  const xml = fs.readFileSync(xmlPath, 'utf8');
  const matches = xml.matchAll(/test-case[^>]*name="([^"]+)"[^>]*result="Failed"/g);
  return [...matches].map(m => m[1]);
}

function reportToSlack(failures) {
  if (!slackWebhook) return;
  if (!isValidSlackWebhookUrl(slackWebhook)) {
    console.error('Invalid SLACK_WEBHOOK_URL (must be HTTPS and slack.com). SSRF prevention.');
    return;
  }
  const text = failures.length > 0
    ? `QA failures: ${failures.slice(0, 5).join(', ')}${failures.length > 5 ? '...' : ''}`
    : 'QA pipeline failed.';
  const url = new URL(slackWebhook);
  const body = JSON.stringify({ text });
  const req = require('https').request({
    hostname: url.hostname,
    path: url.pathname + url.search,
    method: 'POST',
    headers: { 'Content-Type': 'application/json', 'Content-Length': Buffer.byteLength(body) },
  }, () => {});
  req.write(body);
  req.end();
}

function reportToJira(failures) {
  if (!jiraUrl || !jiraToken || failures.length === 0) return;
  if (!isValidJiraUrl(jiraUrl)) {
    console.error('Invalid JIRA_URL (must be HTTPS and atlassian.net). SSRF prevention.');
    return;
  }
  const summary = `QA: ${failures[0]} failed`;
  const body = JSON.stringify({
    fields: {
      project: { key: process.env.JIRA_PROJECT || 'QA' },
      summary,
      description: failures.join('\n'),
      issuetype: { name: 'Bug' },
    },
  });
  const url = new URL(jiraUrl);
  require('https').request({
    hostname: url.hostname,
    path: url.pathname + '/rest/api/3/issue',
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${jiraToken}`,
      'Content-Length': Buffer.byteLength(body),
    },
  }, () => {}).end(body);
}

const failures = parseFailures(xmlPath);
reportToSlack(failures);
reportToJira(failures);
