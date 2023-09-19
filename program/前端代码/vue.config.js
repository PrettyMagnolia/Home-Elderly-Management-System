const IS_PROD = ["production", "prod"].includes(process.env.NODE_ENV);

module.exports = {
  publicPath: IS_PROD ? process.env.VUE_APP_PUBLIC_PATH : "/", // 默认'/'，部署应用包时的基本 URL
  // outputDir: process.env.outputDir || 'dist', // 'dist', 生产环境构建文件的目录
  // assetsDir: "", // 相对于outputDir的静态资源(js、css、img、fonts)目录
  lintOnSave: false,
  runtimeCompiler: true, // 是否使用包含运行时编译器的 Vue 构建版本
  productionSourceMap: !IS_PROD, // 生产环境的 source map
  parallel: require("os").cpus().length > 1,
  pwa: {},


  devServer: {
    // overlay: { // 让浏览器 overlay 同时显示警告和错误
    //   warnings: true,
    //   errors: true
    // },
    // open: false, // 是否打开浏览器
    // host: "localhost",
    port: "80", // 代理断就
    // https: false,
    // hotOnly: false, // 热更新
    proxy: {
        "/API": {
          target:"http://43.142.92.224/api/", // 目标代理接口地址
          secure: false,
          changeOrigin: true, // 开启代理，在本地创建一个虚拟服务端
          // ws: true, // 是否启用websockets
          pathRewrite: {
            "^/API": ''
          }
        }
      }
  }
};