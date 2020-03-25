MRuby::Build.new do |conf|
  # load specific toolchain settings

  # Gets set by the VS command prompts.
  if ENV['VisualStudioVersion'] || ENV['VSINSTALLDIR']
    toolchain :visualcpp
  else
    toolchain :gcc
  end

  #enable_debug

  # Use mrbgems
  # conf.gem 'examples/mrbgems/ruby_extension_example'
  # conf.gem 'examples/mrbgems/c_extension_example' do |g|
  #   g.cc.flags << '-g' # append cflags in this gem
  # end
  # conf.gem 'examples/mrbgems/c_and_ruby_extension_example'
  # conf.gem :core => 'mruby-eval'
  # conf.gem :mgem => 'mruby-io'
  # conf.gem :github => 'iij/mruby-io'
  # conf.gem :git => 'git@github.com:iij/mruby-io.git', :branch => 'master', :options => '-v'

  # include the default GEMs
  conf.gembox 'default'

  conf.gem "#{root}/mrbgems/mruby-bin-dump"
  # C compiler settings
  # conf.cc do |cc|
  #   cc.command = ENV['CC'] || 'gcc'
  #   cc.flags = [ENV['CFLAGS'] || %w()]
  #   cc.include_paths = ["#{root}/include"]
  #   cc.defines = %w(DISABLE_GEMS)
  #   cc.option_include_path = '-I%s'
  #   cc.option_define = '-D%s'
  #   cc.compile_options = "%{flags} -MMD -o %{outfile} -c %{infile}"
  # end

  # mrbc settings
  # conf.mrbc do |mrbc|
  #   mrbc.compile_options = "-g -B%{funcname} -o-" # The -g option is required for line numbers
  # end

  # Linker settings
  # conf.linker do |linker|
  #   linker.command = ENV['LD'] || 'gcc'
  #   linker.flags = [ENV['LDFLAGS'] || []]
  #   linker.flags_before_libraries = []
  #   linker.libraries = %w()
  #   linker.flags_after_libraries = []
  #   linker.library_paths = []
  #   linker.option_library = '-l%s'
  #   linker.option_library_path = '-L%s'
  #   linker.link_options = "%{flags} -o %{outfile} %{objs} %{libs}"
  # end

  # Archiver settings
  # conf.archiver do |archiver|
  #   archiver.command = ENV['AR'] || 'ar'
  #   archiver.archive_options = 'rs %{outfile} %{objs}'
  # end

  # Parser generator settings
  # conf.yacc do |yacc|
  #   yacc.command = ENV['YACC'] || 'bison'
  #   yacc.compile_options = '-o %{outfile} %{infile}'
  # end

  # gperf settings
  # conf.gperf do |gperf|
  #   gperf.command = 'gperf'
  #   gperf.compile_options = '-L ANSI-C -C -p -j1 -i 1 -g -o -t -N mrb_reserved_word -k"1,3,$" %{infile} > %{outfile}'
  # end

  # file extensions
  # conf.exts do |exts|
  #   exts.object = '.o'
  #   exts.executable = '' # '.exe' if Windows
  #   exts.library = '.a'
  # end

  # file separetor
  # conf.file_separator = '/'

  # bintest
  # conf.enable_bintest
end

MRuby::Build.new('host-debug') do |conf|
  # load specific toolchain settings

  # Gets set by the VS command prompts.
  if ENV['VisualStudioVersion'] || ENV['VSINSTALLDIR']
    toolchain :visualcpp
  else
    toolchain :gcc
  end

  enable_debug

  # include the default GEMs
  conf.gembox 'default'

  conf.gem '../mrbgems/mruby-blockly'

  # C compiler settings
  conf.cc.defines = %w(MRB_ENABLE_DEBUG_HOOK)

  # Generate mruby debugger command (require mruby-eval)
  #conf.gem :core => "mruby-bin-debugger"
  conf.gem :core => "mruby-eval"

  # bintest
  # conf.enable_bintest
end
MRuby::Build.new('host-preset') do |conf|
  # load specific toolchain settings

  # Gets set by the VS command prompts.
  if ENV['VisualStudioVersion'] || ENV['VSINSTALLDIR']
    toolchain :visualcpp
  else
    toolchain :gcc
  end

  enable_debug

  # include the default GEMs
  #conf.gembox 'default'

  conf.gem 'mrbgems/mruby-array-ext'
  #conf.gem 'mrbgems/mruby-bin-debugger'
  #conf.gem 'mrbgems/mruby-bin-mirb'
  #conf.gem 'mrbgems/mruby-bin-mrbc'
  #conf.gem 'mrbgems/mruby-bin-mruby'
  #conf.gem 'mrbgems/mruby-bin-mruby-config'
  #conf.gem 'mrbgems/mruby-bin-strip'
  conf.gem 'mrbgems/mruby-class-ext'
  conf.gem 'mrbgems/mruby-compar-ext'
  conf.gem 'mrbgems/mruby-compiler'
  conf.gem 'mrbgems/mruby-enumerator'
  conf.gem 'mrbgems/mruby-enum-ext'
  conf.gem 'mrbgems/mruby-enum-lazy'
  conf.gem 'mrbgems/mruby-error'
  conf.gem 'mrbgems/mruby-eval'
  conf.gem 'mrbgems/mruby-exit'
  conf.gem 'mrbgems/mruby-fiber'
  conf.gem 'mrbgems/mruby-hash-ext'
  conf.gem 'mrbgems/mruby-inline-struct'
  conf.gem 'mrbgems/mruby-io'
  conf.gem 'mrbgems/mruby-kernel-ext'
  conf.gem 'mrbgems/mruby-math'
  conf.gem 'mrbgems/mruby-method'
  conf.gem 'mrbgems/mruby-numeric-ext'
  conf.gem 'mrbgems/mruby-object-ext'
  conf.gem 'mrbgems/mruby-objectspace'
  conf.gem 'mrbgems/mruby-pack'
  conf.gem 'mrbgems/mruby-print'
  conf.gem 'mrbgems/mruby-proc-ext'
  conf.gem 'mrbgems/mruby-random'
  conf.gem 'mrbgems/mruby-range-ext'
  conf.gem 'mrbgems/mruby-socket'
  conf.gem 'mrbgems/mruby-sprintf'
  conf.gem 'mrbgems/mruby-string-ext'
  conf.gem 'mrbgems/mruby-struct'
  conf.gem 'mrbgems/mruby-symbol-ext'
  #conf.gem 'mrbgems/mruby-test'
  conf.gem 'mrbgems/mruby-time'
  conf.gem 'mrbgems/mruby-toplevel-ext'

  conf.gem '../mrbgems/mruby-blockly'

  # C compiler settings
  conf.cc.defines = %w(MRB_ENABLE_DEBUG_HOOK MRB_USE_PRESET_SYMBOLS)

  # bintest
  # conf.enable_bintest
end
=begin
MRuby::Build.new('test') do |conf|
  # Gets set by the VS command prompts.
  if ENV['VisualStudioVersion'] || ENV['VSINSTALLDIR']
    toolchain :visualcpp
  else
    toolchain :gcc
  end

  enable_debug
  conf.enable_bintest
  conf.enable_test

  conf.gembox 'default'
end
=end
#MRuby::Build.new('bench') do |conf|
#  # Gets set by the VS command prompts.
#  if ENV['VisualStudioVersion'] || ENV['VSINSTALLDIR']
#    toolchain :visualcpp
#  else
#    toolchain :gcc
#    conf.cc.flags << '-O3'
#  end
#
#  conf.gembox 'default'
#end

# Define cross build settings
# MRuby::CrossBuild.new('32bit') do |conf|
#   toolchain :gcc
#
#   conf.cc.flags << "-m32"
#   conf.linker.flags << "-m32"
#
#   conf.build_mrbtest_lib_only
#
#   conf.gem 'examples/mrbgems/c_and_ruby_extension_example'
#
#   conf.test_runner.command = 'env'
# end
=begin
MRuby::CrossBuild.new("emscripten") do |conf|
  toolchain :emscripten

  conf.archiver.command = "llvm-ar"
  conf.linker.flags << '--save-bc %{outfile}.bc --pre-js ../test/webmrbc/pre.js --post-js ../test/webmrbc/post.js --use-preload-plugins'

  enable_debug

  # include the default GEMs
  conf.gembox 'default'
  conf.gem :core => 'mruby-bin-mrbc'

  conf.gem '../mrbgems/mruby-blockly'
  conf.gem '../mrbgems/mruby-io'
  conf.gem '../mrbgems/mruby-pack'
  conf.gem '../mrbgems/mruby-socket'

  # C compiler settings
  conf.cc.defines = %w(MRB_ENABLE_DEBUG_HOOK)

  # Generate mruby debugger command (require mruby-eval)
  conf.gem :core => "mruby-bin-debugger"
end

MRuby::CrossBuild.new("emscripten_min") do |conf|
  toolchain :emscripten

  conf.cc do |cc|
    cc.flags[0].delete_at(cc.flags[0].rindex("-g"))
    cc.flags[1].delete_at(cc.flags[1].rindex("-O0"))
    cc.flags << "-Oz"
  end

  conf.cxx do |cxx| 
    cxx.flags[0].delete_at(cxx.flags[0].rindex("-g"))
    cxx.flags[1].delete_at(cxx.flags[1].rindex("-O0"))
    cxx.flags << "-Oz"
  end

  conf.gembox 'default'

  conf.gem :core => 'mruby-bin-mrbc'

  conf.linker.flags << '--save-bc %{outfile}.bc --pre-js ../test/webmrbc/pre.js --post-js ../test/webmrbc/post.js --use-preload-plugins --closure 1'
end
=end
