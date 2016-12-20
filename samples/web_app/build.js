var path = require("path");
var fs = require("fs-extra");
var fable = require("fable-compiler");

var version;

var targets = {
  all() {
    return fable.promisify(fs.remove, "public/js")
      .then(_ => fable.compile({ projFile: "./" }))
  },
  watch() {
    return fable.promisify(fs.remove, "public/js")
      .then(_ => fable.compile({ projFile: "./" , watch: true}))
  }
  // publish() {
  //   return this.all()
  //     .then(_ => {
  //       var command = version && version.indexOf("alpha") > -1
  //         ? "npm publish --tag next"
  //         : "npm publish";
  //       fable.runCommand("npm", command)
  //     })
  // }
}

targets[process.argv[2] || "all"]().catch(err => {
  console.log("[ERROR] " + err);
  process.exit(-1);
});
