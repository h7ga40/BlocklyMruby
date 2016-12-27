MRuby::Toolchain.new(:ca850) do |conf|
  [conf.cc, conf.cxx].each do |cc|
    cc.command = ENV['CC'] || 'ca850.exe'
    cc.flags = [ENV['CFLAGS'] || %w(-c -g -Xcxxcom -Xdefvar -Xpro_epi_runtime=off)]
    cc.include_paths = ["#{MRUBY_ROOT}\\include", "#{MRUBY_ROOT}\\..\\csp\\ca850"]
    cc.defines = %w(DISABLE_GEMS)
    cc.option_include_path = '-I%s'
    cc.option_define = '-D%s'
    cc.compile_options = "-cpu f3783 \"-devpath=#{DEVICE_FILE_PATH}\" %{infile} -o %{outfile} %{flags}"
  end

  conf.linker do |linker|
    linker.command = ENV['LD'] || 'ld850.exe'
    linker.flags = [ENV['LDFLAGS']]
    linker.flags_before_libraries = '-Xsid=0xffffffffffffffffffff -rc'
    linker.libraries = %w(m c r)
    linker.library_paths = ["#{LIBRARIES_PATH}"]
    linker.option_library = '-l%s'
    linker.option_library_path = '-L%s'
    linker.link_options = "-cpu f3783 -F \"#{DEVICE_FILE_PATH}\" %{objs} -o %{outfile} %{flags} %{flags_before_libraries} %{libs} %{flags_after_libraries}"
  end
 
  conf.archiver do |archiver|
    archiver.command = ENV['AR'] || 'ar850.exe'
    archiver.archive_options = 'q %{outfile} %{objs}'
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
    exts.object = '.o'
    exts.executable = '.out'
    exts.library = '.a'
  end

  conf.file_separator = '\\'
end
