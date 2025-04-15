# Extensions

This section describes the different extensions supported:

## AutoLinks

Autolinks will format as a HTML link any string that starts by:

- `http://` or `https://` 
- `ftp://`
- `mailto:`
- `tel:`
- `www.` 
 
```````````````````````````````` example
This is a http://www.google.com URL and https://www.google.com
This is a ftp://test.com
And a mailto:email@toto.com
And a tel:+1555123456
And a plain www.google.com
.
<p>This is a <a href="http://www.google.com">http://www.google.com</a> URL and <a href="https://www.google.com">https://www.google.com</a>
This is a <a href="ftp://test.com">ftp://test.com</a>
And a <a href="mailto:email@toto.com">email@toto.com</a>
And a <a href="tel:+1555123456">+1555123456</a>
And a plain <a href="http://www.google.com">www.google.com</a></p>
````````````````````````````````

But incomplete links will not be matched:
 
```````````````````````````````` example
This is not a http:/www.google.com URL and https:/www.google.com
This is not a ftp:/test.com
And not a mailto:emailtoto.com
And not a plain www. or a www.x 
And not a tel:
.
<p>This is not a http:/www.google.com URL and https:/www.google.com
This is not a ftp:/test.com
And not a mailto:emailtoto.com
And not a plain www. or a www.x
And not a tel:</p>
````````````````````````````````

Previous character must be a punctuation or a valid space (tab, space, new line):
 
```````````````````````````````` example
This is not a nhttp://www.google.com URL but this is (https://www.google.com)
.
<p>This is not a nhttp://www.google.com URL but this is (<a href="https://www.google.com">https://www.google.com</a>)</p>
````````````````````````````````

An autolink should not interfere with an `<a>` HTML inline:
 
```````````````````````````````` example
This is an HTML <a href="http://www.google.com">http://www.google.com</a> link
.
<p>This is an HTML <a href="http://www.google.com">http://www.google.com</a> link</p>
````````````````````````````````
or even within emphasis:
 
```````````````````````````````` example
This is an HTML <a href="http://www.google.com"> **http://www.google.com** </a> link
.
<p>This is an HTML <a href="http://www.google.com"> <strong>http://www.google.com</strong> </a> link</p>
````````````````````````````````


An autolink should not interfere with a markdown link:
 
```````````````````````````````` example
This is an HTML [http://www.google.com](http://www.google.com) link
.
<p>This is an HTML <a href="http://www.google.com">http://www.google.com</a> link</p>
````````````````````````````````

A link embraced by pending emphasis should let the emphasis takes precedence if characters are placed at the end of the matched link:
 
```````````````````````````````` example
Check **http://www.a.com** or __http://www.b.com__
.
<p>Check <strong><a href="http://www.a.com">http://www.a.com</a></strong> or <strong><a href="http://www.b.com">http://www.b.com</a></strong></p>
````````````````````````````````

It is not mentioned by the spec, but empty emails won't be matched (only a subset of [RFC2368](https://tools.ietf.org/html/rfc2368) is supported by auto links):

```````````````````````````````` example
mailto:email@test.com is okay, but mailto:@test.com is not
.
<p><a href="mailto:email@test.com">email@test.com</a> is okay, but mailto:@test.com is not</p>
````````````````````````````````

### GFM Support

Extract from [GFM Autolinks extensions specs](https://github.github.com/gfm/#autolinks-extension-)

```````````````````````````````` example
www.commonmark.org
.
<p><a href="http://www.commonmark.org">www.commonmark.org</a></p>
````````````````````````````````

```````````````````````````````` example
Visit www.commonmark.org/help for more information.
.
<p>Visit <a href="http://www.commonmark.org/help">www.commonmark.org/help</a> for more information.</p>
````````````````````````````````

```````````````````````````````` example
Visit www.commonmark.org.

Visit www.commonmark.org/a.b.
.
<p>Visit <a href="http://www.commonmark.org">www.commonmark.org</a>.</p>
<p>Visit <a href="http://www.commonmark.org/a.b">www.commonmark.org/a.b</a>.</p>
````````````````````````````````


```````````````````````````````` example
www.google.com/search?q=Markup+(business)

(www.google.com/search?q=Markup+(business))
.
<p><a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a></p>
<p>(<a href="http://www.google.com/search?q=Markup+(business)">www.google.com/search?q=Markup+(business)</a>)</p>
````````````````````````````````


```````````````````````````````` example
www.google.com/search?q=commonmark&hl=en

www.google.com/search?q=commonmark&hl;
.
<p><a href="http://www.google.com/search?q=commonmark&amp;hl=en">www.google.com/search?q=commonmark&amp;hl=en</a></p>
<p><a href="http://www.google.com/search?q=commonmark">www.google.com/search?q=commonmark</a>&amp;hl;</p>
````````````````````````````````


```````````````````````````````` example
www.commonmark.org/he<lp
.
<p><a href="http://www.commonmark.org/he">www.commonmark.org/he</a>&lt;lp</p>
````````````````````````````````

```````````````````````````````` example
http://commonmark.org

(Visit https://encrypted.google.com/search?q=Markup+(business))

Anonymous FTP is available at ftp://foo.bar.baz.
.
<p><a href="http://commonmark.org">http://commonmark.org</a></p>
<p>(Visit <a href="https://encrypted.google.com/search?q=Markup+(business)">https://encrypted.google.com/search?q=Markup+(business)</a>)</p>
<p>Anonymous FTP is available at <a href="ftp://foo.bar.baz">ftp://foo.bar.baz</a>.</p>
````````````````````````````````

### Valid Domain Tests

Domain names that have empty segments won't be matched

```````````````````````````````` example
www..
www..com
http://test.
http://.test
http://.
http://..
ftp://test.
ftp://.test
mailto:email@test.
mailto:email@.test
.
<p>www..
www..com
http://test.
http://.test
http://.
http://..
ftp://test.
ftp://.test
mailto:email@test.
mailto:email@.test</p>
````````````````````````````````

Domain names with too few segments won't be matched

```````````````````````````````` example
www
www.com
http://test
ftp://test
mailto:email@test
.
<p>www
www.com
http://test
ftp://test
mailto:email@test</p>
````````````````````````````````

Domain names that contain an underscores in the last two segments won't be matched

```````````````````````````````` example
www._test.foo.bar is okay, but www._test.foo is not

http://te_st.foo.bar is okay, as is http://test.foo_.bar.foo

But http://te_st.foo, http://test.foo_.bar and http://test._foo are not

ftp://test_.foo.bar is okay, but ftp://test.fo_o is not

mailto:email@_test.foo.bar is okay, but mailto:email@_test.foo is not
.
<p><a href="http://www._test.foo.bar">www._test.foo.bar</a> is okay, but www._test.foo is not</p>
<p><a href="http://te_st.foo.bar">http://te_st.foo.bar</a> is okay, as is <a href="http://test.foo_.bar.foo">http://test.foo_.bar.foo</a></p>
<p>But http://te_st.foo, http://test.foo_.bar and http://test._foo are not</p>
<p><a href="ftp://test_.foo.bar">ftp://test_.foo.bar</a> is okay, but ftp://test.fo_o is not</p>
<p><a href="mailto:email@_test.foo.bar">email@_test.foo.bar</a> is okay, but mailto:email@_test.foo is not</p>
````````````````````````````````

Domain names that contain invalid characters (not AlphaNumberic, -, _ or .) won't be matched

```````````````````````````````` example
https://[your-domain]/api
.
<p>https://[your-domain]/api</p>
````````````````````````````````

Domain names followed by ?, : or # instead of / are matched

```````````````````````````````` example
https://github.com?

https://github.com?a

https://github.com#a

https://github.com:

https://github.com:443
.
<p><a href="https://github.com">https://github.com</a>?</p>
<p><a href="https://github.com?a">https://github.com?a</a></p>
<p><a href="https://github.com#a">https://github.com#a</a></p>
<p><a href="https://github.com">https://github.com</a>:</p>
<p><a href="https://github.com:443">https://github.com:443</a></p>
````````````````````````````````

### Unicode support

Links with unicode characters in the path / query / fragment are matched and url encoded

```````````````````````````````` example
http://abc.net/☃

http://abc.net?☃

http://abc.net#☃

http://abc.net/foo#☃
.
<p><a href="http://abc.net/%E2%98%83">http://abc.net/☃</a></p>
<p><a href="http://abc.net?%E2%98%83">http://abc.net?☃</a></p>
<p><a href="http://abc.net#%E2%98%83">http://abc.net#☃</a></p>
<p><a href="http://abc.net/foo#%E2%98%83">http://abc.net/foo#☃</a></p>
````````````````````````````````

Unicode characters in the FQDN are matched and IDNA encoded

```````````````````````````````` example
http://☃.net?☃
.
<p><a href="http://xn--n3h.net?%E2%98%83">http://☃.net?☃</a></p>
````````````````````````````````

Same goes for regular autolinks

```````````````````````````````` example
<http://abc.net/☃>

<http://abc.net?☃>

<http://abc.net#☃>

<http://abc.net/foo#☃>
.
<p><a href="http://abc.net/%E2%98%83">http://abc.net/☃</a></p>
<p><a href="http://abc.net?%E2%98%83">http://abc.net?☃</a></p>
<p><a href="http://abc.net#%E2%98%83">http://abc.net#☃</a></p>
<p><a href="http://abc.net/foo#%E2%98%83">http://abc.net/foo#☃</a></p>
````````````````````````````````

```````````````````````````````` example
<http://☃.net?☃>
.
<p><a href="http://xn--n3h.net?%E2%98%83">http://☃.net?☃</a></p>
````````````````````````````````

It also complies with CommonMark's vision of priority.
This will therefore be seen as an autolink and not as code inline.

```````````````````````````````` example
<http://foö.bar.`baz>`
.
<p><a href="http://xn--fo-gka.bar.%60baz">http://foö.bar.`baz</a>`</p>
````````````````````````````````

Unicode punctuation characters are not allowed, but symbols are.
Note that this does _not_ exactly match CommonMark's "Unicode punctuation character" definition.

```````````````````````````````` example
http://☃.net?☃ // OtherSymbol

http://🍉.net?🍉 // A UTF-16 surrogate pair, but code point is OtherSymbol

http://‰.net?‰ // OtherPunctuation
.
<p><a href="http://xn--n3h.net?%E2%98%83">http://☃.net?☃</a> // OtherSymbol</p>
<p><a href="http://xn--ji8h.net?%F0%9F%8D%89">http://🍉.net?🍉</a> // A UTF-16 surrogate pair, but code point is OtherSymbol</p>
<p>http://‰.net?‰ // OtherPunctuation</p>
````````````````````````````````