# Extensions

This section describes the different extensions supported:

## Emoji

Emoji shortcodes and smileys can be converted to their respective unicode characters:

```````````````````````````````` example
This is a test with a :) and a :angry: smiley
.
<p>This is a test with a 😃 and a 😠 smiley</p>
````````````````````````````````

An emoji needs to be preceded by a space:

```````````````````````````````` example
These are not:) an emoji with a:) x:angry:x
.
<p>These are not:) an emoji with a:) x:angry:x</p>
````````````````````````````````

Emojis can be followed by close punctuation (or any other characters):

```````````````````````````````` example
We all need :), it makes us :muscle:. (and :ok_hand:).
.
<p>We all need 😃, it makes us 💪. (and 👌).</p>
````````````````````````````````

Sentences can end with emojis:

```````````````````````````````` example
This is a sentence :ok_hand:
and keeps going to the next line :)
.
<p>This is a sentence 👌
and keeps going to the next line 😃</p>
````````````````````````````````
