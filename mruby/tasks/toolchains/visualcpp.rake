MRuby::Toolchain.new(:visualcpp) do |conf, _params|
  conf.cc do |cc|
    cc.command = ENV['CC'] || 'cl.exe'
    cc.flags = [ENV['CFLAGS'] || %w(/c /nologo /W4 /GS /analyze- /Zc:wchar_t /ZI /Gm /Od /Zc:inline /fp:precise /errorReport:prompt /WX- /Zc:forScope /RTC1 /Gd /Oy- /MDd /EHsc /DWIN32 /D_DEBUG /D_CONSOLE /D_CRT_SECURE_NO_WARNINGS)]
    cc.flags << %w(/we4002 /we4003 /we4005 /we4007 /we4010 /we4013 /we4015 /we4020 /we4022 /we4024 /we4028 /we4029 /we4031 /we4033 /we4034 /we4042 /we4047 /we4048 /we4049 /we4056 /we4067 /we4074 /we4079 /we4083 /we4088 /we4089 /we4090 /we4091 /we4094 /we4096 /we4098 /we4099 /we4113 /we4133 /we4715 /we4716)
    cc.flags << %w(/wd4214 /wd4100 /wd4996)
    cc.flags << "/Fd\"#{conf.build_dir}\\vc140.pdb\""
    cc.flags << "/Fp\"%{outdir}\\%{outfilebase}.pch\""
    cc.defines = %w(DISABLE_GEMS MRB_STACK_EXTEND_DOUBLING)
    cc.option_include_path = '/I%s'
    cc.option_define = '/D%s'
    cc.compile_options = "%{flags} /Fo%{outfile} %{infile}"
    cc.cxx_compile_flag = '/TP'
    cc.cxx_exception_flag = '/EHs'
  end

  conf.cxx do |cxx|
    cxx.command = ENV['CXX'] || 'cl.exe'
    cxx.flags = [ENV['CXXFLAGS'] || ENV['CFLAGS'] || %w(/c /nologo /W4 /GS /analyze- /Zc:wchar_t /ZI /Gm /Od /Zc:inline /fp:precise /errorReport:prompt /WX- /Zc:forScope /RTC1 /Gd /Oy- /MDd /EHsc /DWIN32 /D_DEBUG /D_CONSOLE /D_CRT_SECURE_NO_WARNINGS)]
    cxx.flags << %w(/we4002 /we4003 /we4005 /we4007 /we4010 /we4013 /we4015 /we4020 /we4022 /we4024 /we4028 /we4029 /we4031 /we4033 /we4034 /we4042 /we4047 /we4048 /we4049 /we4056 /we4067 /we4074 /we4079 /we4083 /we4088 /we4089 /we4090 /we4091 /we4094 /we4096 /we4098 /we4099 /we4113 /we4133 /we4715 /we4716)
    cxx.flags << %w(/wd4214 /wd4100 /wd4996)
    cxx.flags << "/Fd\"#{conf.build_dir}\\vc140.pdb\""
    cxx.flags << "/Fp\"%{outdir}\\%{outfilebase}.pch\""
    cxx.defines = %w(DISABLE_GEMS MRB_STACK_EXTEND_DOUBLING)
    cxx.option_include_path = '/I%s'
    cxx.option_define = '/D%s'
    cxx.compile_options = "%{flags} /Fo%{outfile} %{infile}"
    cxx.cxx_compile_flag = '/TP'
    cxx.cxx_exception_flag = '/EHs'
  end

  conf.linker do |linker|
    linker.command = ENV['LD'] || 'link.exe'
    linker.flags = [ENV['LDFLAGS'] || %w(/MANIFEST /NXCOMPAT /DYNAMICBASE /MACHINE:X86 /INCREMENTAL /SUBSYSTEM:CONSOLE /MANIFESTUAC:"level='asInvoker' uiAccess='false'" /ERRORREPORT:PROMPT /NOLOGO /TLBID:1)]
    linker.flags << "/PDB:\"#{conf.build_dir}\\vc140.pdb\""
    #linker.flags << "/PGD:\"%{outdir}\\%{outfilebase}.pgd\""
    linker.flags << "/ManifestFile:\"%{outdir}\\%{outfilebase}.exe.intermediate.manifest\""
    linker.libraries = %w()
    linker.library_paths = %w()
    linker.option_library = '%s.lib'
    linker.option_library_path = '/LIBPATH:%s'
    linker.link_options = "%{flags} /OUT:%{outfile} %{objs} %{flags_before_libraries} %{libs} %{flags_after_libraries}"
  end

  conf.archiver do |archiver|
    archiver.command = ENV['AR'] || 'lib.exe'
    archiver.archive_options = '/nologo /OUT:%{outfile} %{objs}'
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
    exts.executable = '.exe'
    exts.library = '.lib'
  end

  conf.file_separator = '\\'

  # Unreliable detection and will result in invalid encoding errors for localized versions of Visual C++
  # if require 'open3'
  #   Open3.popen3 conf.cc.command do |_, _, e, _|
  #     if /Version (\d{2})\.\d{2}\.\d{5}/ =~ e.gets && $1.to_i <= 17
  #       m = "# VS2010/2012 support will be dropped after the next release! #"
  #       h = "#" * m.length
  #       puts h, m, h
  #     end
  #   end
  # end

end
