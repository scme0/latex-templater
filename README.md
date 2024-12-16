# Tmpltr

🚧 Work in Progress 🚧 This is a new project which isn't fully implemented yet 🙂

Tmpltr is a small app that allows you to convert any text-based file type into a template where you can inject data from a yaml.

The original usecase is for a latex resume where you want to extract the actual information out of latex so that it's easier to edit and update.

The three ways it works are:
1. Specify a property and it will directly inject the value into the document as a string.
2. Specify a property that is an object and specify another template file:
  a. Tmpltr will load the second template and evaluate it against the property's object. It will then inject the output of the evaluation in-line.
3. Specify a property that is an array and specify another template file:
  a. Tmpltr will load the second template and evaluate it against each object in the array. It will then inject the output of each evaluation on a new line.
  b. If an array is just a list of strings, you can refence the string value in the subtemplate with `<< . >>`

Format:
1. `<< path.to.property >>`
2. and 3. `<< path.to.property | path/to/subtemplate.tex >>`

CommandLine args:

`tmpltr [-d '{{ }}'] path/to/basetemplate.tex path/to/data.yaml [-o path/to/output.tex]`

`-o` is to specify an output file. If you don't specify it, the result will be sent to stdout.

`-d` is to specify the injection marker for properties. The default is '<< >>' but you can specify anything as long as the end is different to the start (eg: NOT `$$ $$`). The format to put in the command line arg is `<start><space><end>`. It can be longer than 2 chars. eg: `~:> <:~`
