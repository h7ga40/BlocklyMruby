del MrbParser.output
del MrbParser.trace

..\CSYacc\bin\Debug\CSYacc -c -t -v mrb_parse.jay skeleton.cs > MrbParser.cs
rem ..\CSYacc\bin\Debug\CSYacc -t -v mrb_parse.jay skeleton.cs > MrbParser.cs
ren y.output MrbParser.output
ren y.trace MrbParser.trace

