const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const context =  [
  "/weatherforecast",
  "/_configuration",
  "/.well-known",
  "/Identity",
  "/connect",
  "/ApplyDatabaseMigrations",
  "/_framework",
  "/weatherforecast"
];

module.exports = function(app) {
  const appProxy = createProxyMiddleware(context, {
      target: 'https://localhost:5031',
    secure: false,
    headers: {
      Connection: 'Keep-Alive'
    },
    router: {
      '/weatherforecast': 'https://localhost:5011'
    }
  });

  app.use(appProxy);
};
