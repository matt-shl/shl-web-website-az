// const path = require('path')

const extendConfig = config => {
  // config.copy = [
  //   ...config.copy,
  //   {
  //     from: path.resolve(config.assets, 'videos'),
  //     to: 'assets/videos',
  //   }
  // ]

  config.htmlOutputPath = config.buildStatic || config.isDevelopment ? '' : 'html'

  return config
}

module.exports = extendConfig
