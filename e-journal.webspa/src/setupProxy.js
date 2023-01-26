const { createProxyMiddleware } = require('http-proxy-middleware');

const context =  [
  "/weatherforecast",
  "/_configuration",
  "/.well-known",
  "/Identity",
  "/connect",
  "/ApplyDatabaseMigrations",
  "/_framework",
  "/schedules",
  "/journal"
];

module.exports = function(app) {
  const appProxy = createProxyMiddleware(context, {
      target: 'https://localhost:5031',
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    },
    router: {
      '/schedules': 'https://localhost:5011/api',
      '/journal': 'https://localhost:5021/api'
    },
    pathRewrite: {
      '/journal': ''
    }
  });

  app.use(appProxy);
};
