MRuby::Toolchain.new(:emscripten) do |conf|
  toolchain :gcc

  conf.cc do |cc|
    cc.command = "emcc"
    cc.flags.push(%w(-Wall -Werror-implicit-function-declaration -Wno-warn-absolute-paths -O0))
    cc.flags[0].delete_at(cc.flags[0].rindex("-Og"))
  end

  conf.cxx do |cxx| 
    cxx.command = "emcc"
    cxx.flags.push(%w(-Wall -Werror-implicit-function-declaration -Wno-warn-absolute-paths -O0))
    cxx.flags[0].delete_at(cxx.flags[0].rindex("-Og"))
  end

  conf.linker.command = "emcc"
  conf.archiver.command = "emar"
  conf.exts.executable = '.js'
end
