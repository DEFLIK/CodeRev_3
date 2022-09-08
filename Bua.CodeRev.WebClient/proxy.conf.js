PROXY_CONFIG = [
    {
      context: [
        "/api/auth/*",
      ],
      target: "https://localhost:44393/",
      secure: false,
    },
    {
        context: [
          "/api/compile/*",
        ],
        target: "https://localhost:44395/",
        secure: false,
        logLevel: 'debug'
    }
];
  
module.exports = PROXY_CONFIG;