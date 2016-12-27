MRuby::Toolchain.new(:ccrx) do |conf|
  [conf.cc, conf.cxx].each do |cc|
    cc.command = ENV['CC'] || 'ccrx.exe'
    cc.flags = [ENV['CFLAGS'] || %w(-debug -fpu -nologo)]
    cc.include_paths = ["#{MRUBY_ROOT}\\include"]
    cc.defines = %w(DISABLE_GEMS)
    cc.option_include_path = '-include=%s'
    cc.option_define = '-define=%s'
    cc.compile_options = "%{infile} -isa=rxv1 -lang=c99 -output=obj=%{outfile} %{flags}"
  end

  conf.linker do |linker|
    linker.command = ENV['LD'] || 'rlink.exe'
    linker.flags = [ENV['LDFLAGS']]
    linker.flags_before_libraries = ''
    linker.libraries = %w(m c r)
    linker.option_library = '-library=%s'
    linker.option_library_path = ''
    linker.link_options = "-nologo -form=Absolute %{flags} %{objs} -output=%{outfile} %{flags_before_libraries} %{libs} %{flags_after_libraries}"
  end

  conf.archiver do |archiver|
    archiver.command = ENV['AR'] || 'rlink.exe'
    archiver.archive_options = '-nologo -form=library -noprelink -nooptimize -output=%{outfile} %{objs}'
  end

  conf.yacc do |yacc|
    yacc.command = ENV['YACC'] || 'bison.exe'
    yacc.compile_options = '-o %{outfile} %{infile}'
  end

  conf.gperf do |gperf|
    gperf.command = 'gperf.exe'
    gperf.compile_options = '-L ANSI-C -C -p -j1 -i 1 -g -o -t -N mrb_reserved_word -k"1,3,$" %{infile} > %{outfile}'
  end

  conf.exts do |exts|
    exts.object = '.obj'
    exts.executable = '.abs'
    exts.library = '.lib'
  end

  conf.file_separator = '\\'
end
