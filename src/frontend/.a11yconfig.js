module.exports = {
  maxWorkers: 4,
  takeScreenshot: true,
  fileIgnore: [
    // Ignore the automatically generated pages list "index.html" by default
    '**/index.html',
  ],
  // For more documentation on usage check
  // https://www.npmjs.com/package/pa11y#configuration
  pa11yConfig: {
    runners: ['axe'],
    standard: 'WCAG2AA',
    includeNotices: true,
    includeWarnings: true,
    // For possible options see: https://dequeuniversity.com/rules/axe/html/4.7 and the other versions
    // Example: "duplicate-id" for ignoring duplicate IDs
    ignore: [],
    viewport: {
      width: 1920,
      height: 1080, // Height will be overridden to always match the document height
    },
  },
}
