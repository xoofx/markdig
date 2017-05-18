## Jira Links

The JiraLinks extension will automatically add links to JIRA issue items within your markdown, e.g. XX-1234. For this to happen, you must configure the extension when adding to the pipeline, e.g. 

```
var pipeline = new MarkdownPipelineBuilder()
	.UseJiraLinks(new JiraLinkOptions()
	{
		Url = "https://company.atlassian.net"
	})
	.Build();
```

The project key must be capitalised for the link to be detected, it can be any length. The following are valid examples. 

* EG-123
* TEST-123
* DIG-123

A single hypen must separate the project key and issue number. The issue number can be any length.

The reference must be preceeded by either nothing or whitespace.

The reference must be followed by either nothing or whitespace.