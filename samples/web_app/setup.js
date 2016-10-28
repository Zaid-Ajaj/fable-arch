var cpx = require("cpx");

var FableArchDest = "src";

// Copy Fable Arch files
cpx.copy("node_modules/fable-arch/Fable.Arch.App.fs", FableArchDest, { clean: true });
cpx.copy("node_modules/fable-arch/Fable.Arch.Html.fs", FableArchDest, { clean: true });
cpx.copy("node_modules/fable-arch/Fable.Arch.RouteParser.fs", FableArchDest, { clean: true });
cpx.copy("node_modules/fable-arch/Fable.Arch.VirtualDom.fs", FableArchDest, { clean: true });
