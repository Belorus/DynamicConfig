# DynamicConfig

Sometimes during development of mobile apps you encounter a situation when you want to have some configurable options but still don't want to have a server or have not time to setup a backoffice for all that stuff.
A file like settings.json put somewhere to dropbox/whatever free file hosting can solve your problem.

This library can help you.

Here is a sample from my application:
    
    config:
        version:                            "2.18.0"
    
    http:
        -DEV-timeout:                       30
        timeout:                            10
        -Android-reconnection_strategy:     Default
        reconnection_strategy:              Default
    
    resources:
        zip_header_verification:            false
        -DEV-zip_header_verification:       true
    
    auto_login:
        timeout:                            30
        -2110-timeout:                      30
        force_relogin:                      0
        show_force_relogin_popup:           false


YAML is a great format for configs. It is easily readable, has some nice things (like multiline strings, variables, templates, and much more [on wiki](https://en.wikipedia.org/wiki/YAML).

Api of the library is pretty simple.

    Stream downloadedConfig = DownloadConfig(configUrl);
    var config = DynamicConfigFactory.CreateConfig(downloadedConfig);
    config.SetPrefixes("DEV", "Android");
    config.Build();

Now we can use it like this:

    int httpTimeout = config.Get<int>("http:timeout");

   
We have initialized config with 2 prefixes: DEV and Android. This means that when library finds a key in prefixed by DEV it will prefer it over key without prefix. If it finds key prefixed with both prefixes - it will prefer it over the on prefixed by only one of them.

More on wiki https://github.com/Belorus/DynamicConfig/wiki

Copyright (c) 20015 Maxim Bozbey

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
