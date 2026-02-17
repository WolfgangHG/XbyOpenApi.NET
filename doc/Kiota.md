# OpenAPI client with Kiota

This project uses [Kiota](https://github.com/microsoft/kiota/) to generate a REST client from the OpenAPI specification of the X services.
The spec is found at https://api.twitter.com/2/openapi.json


# Table of Contents

- [Installing Kiota](#installing-kiota)
- [Generating the client](#generating-the-client)
- [Initializing the project](#initializing-the-project)
- [Using the client](#using-the-client)
- [Hack: logging requests and responses](#hack-logging-requests-and-responses)
- [Kiota trouble: uploading binary data](#kiota-trouble-uploading-binary-data)
- [What about other OpenAPI libraries?](#what-about-other-openapi-libraries)


# Installing Kiota
First of all, we have to install the tool `Microsoft.OpenApi.Kiota` (see https://learn.microsoft.com/en-us/openapi/kiota/install)

This could be done globally, but I prefer to do it locally in my solution/project. So, in the root of the project where 
the client should be placed, call this:

```
dotnet tool install Microsoft.OpenApi.Kiota
```

This will create a file `dotnet-tools.json` with this content:

```json
{
  "version": 1,
  "isRoot": true,
  "tools": {
    "microsoft.openapi.kiota": {
      "version": "1.30.0",
      "commands": [
        "kiota"
      ],
      "rollForward": false
    }
  }
}
```

# Generating the client

We tell Kiota to pull the OpenAPI file directly from X. I would prefer to add the file to my repository for being able to compare it,
but this is probably against copyright rules.


In my sample, the client is placed in a subdir "Client", and the namespace is thus `XbyOpenApi.Core.Client`.  The class shall be `XClient`.
So we can create the client with this command:

```
dotnet kiota generate -l CSharp -c XClient -n XbyOpenApi.Core.Client -d https://api.twitter.com/2/openapi.json -o ./Client --exclude-backward-compatible
```

The last argument `--exclude-backward-compatible` is set because we don't need backwards compatible code. Omitting it might
cause different client code.

# Initializing the project

Add this Nuget package reference to client project to make it compile:

```xml
  <ItemGroup>
    <PackageReference Include="Microsoft.Kiota.Bundle" Version="1.21.2" />
  </ItemGroup>
```

# Using the client

This could be simple. For example this snippet should fetch the data of the current user:

```c#
XClient client = new XClient();

Get2UsersMeResponse response = await client.Two.Users.Me.GetAsync();
```

But we need authorization for each API call. This is described in different sections [OAuth1](OAuth1.md) and [OAuth2](OAuth2.md).


# Hack: logging requests and responses

Unfortunately, Kiota does not provide an easy way log all requests and responses with a simple code snippet.

I found [this](https://github.com/microsoft/kiota-dotnet/issues/482) Kiota issue which pointed me to the concept of a 
body inspection handler. This handler must be registered for each request.

For example to view the raw response from the call to fetch the current user data:


```c#
XClient client = new XClient();

var requestOption = new BodyInspectionHandlerOption { InspectResponseBody = true };

Get2UsersMeResponse response = await client.Two.Users.Me.GetAsync(conf =>
{
  conf.Options.Add(requestOption);
}

string plainResponse = GetStringFromStream(requestOption.ResponseBody);
```


The helper method `GetStringFromStream` is defined like this:

```c#
private static string GetStringFromStream(Stream stream)
{
  var reader = new StreamReader(stream);
  using (reader)
  {
    return reader.ReadToEnd();
  }
}
```

The same can be done for inspecting the request. Here is a sample for posting a tweet:

```c#
var requestOption = new BodyInspectionHandlerOption { InspectRequestBody = true, InspectResponseBody = true };

TweetCreateRequest body = new TweetCreateRequest();
body.Text = "Sample post";

TweetCreateResponse response = await xClient.Two.Tweets.PostAsync(body, conf =>
{
  conf.Options.Add(requestOption);
});

string response = GetStringFromStream(requestOption.ResponseBody);
string request = GetStringFromStream(requestOption.RequestBody);
```


This trick might be helpful for error handling. Normally, Kiota creates error objects that contain the error message object from the service.
If you try to delete a tweet with an id that you dont' have write access, the service response message is this:

```json
{
  "detail": "You are not authorized to delete this Tweet.",
  "type": "about:blank",
  "title": "Forbidden",
  "status": 403
}
```

This matches the generated model class "Problem", so it should be thrown and you would see a helpful error. 
But unfortunately, Kiota throws an "Error" instance instance, which has different fields
and thus cannot be filled with the response data - the thrown error seems to be invalid.


Here, two problems come together: 
* Problem 1: the endpoint to delete a tweet currently apparently has a wrong definition
in OpenAPI version 2.157: it should return an object "Error" if the response content type is "application/json" and 
"Problem" if the content type is "application/problem+json".
But with an invalid tweet id it returns a "Problem" object and the content type is "application/json". When invoking the endpoint with an
invalid OAuth2 access token, it returns also a "Problem" object, but with content type "application/problem+json".
Kiota tries to parse the wrong error object and thus throws an empty (and meaningless) error message.
* Problem 2 (would be relevant if the X specification would be valid): I think Kiota cannot handle different results 
based on the content type at all, so I created issue https://github.com/microsoft/kiota/issues/7338

To see the actual error message, I added the `BodyInspectionHandlerOption` and parsed the request in the catch block,
which revealed the actual error.

# Kiota trouble: uploading binary data

Handling of binary data does not work with at least one endpoint of the Kiota generated client.
The endpoint to upload media (https://docs.x.com/x-api/media/upload-media) is created with unusable code.

It could work like this using the generated classes:

```c#
byte[] data = ...;
MediaUploadRequestOneShot mediaUpload = new MediaUploadRequestOneShot();
mediaUpload.MediaCategory = MediaCategoryOneShot.Tweet_image;
mediaUpload.Media = new MediaUploadRequestOneShot.MediaUploadRequestOneShot_media();
mediaUpload.Media.MediaPayloadByte = new MediaPayloadByte();

await xClient.Two.Media.Upload.PostAsync(mediaUpload);
```

But there is no way to add the binary data of a media. The class `MediaPayloadByte` has no properties at all.

I asked this in the Kiota project, an answer is pending: https://github.com/microsoft/kiota/discussions/7247 and https://github.com/microsoft/kiota/issues/7394

But I hope I found a workaround.

This is the generated class (slightly simplified to remove e.g. namespaces and code that is not relevant here):
```c#
public partial class MediaUploadRequestOneShot : IParsable
{
  public MediaUploadRequestOneShot.MediaUploadRequestOneShot_media Media { get; set; }
  public MediaCategoryOneShot? MediaCategory { get; set; }
  public MediaUploadRequestOneShot_media_type? MediaType { get; set; }
  public bool? Shared { get; set; }
  
  public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
  {
    return new Dictionary<string, Action<IParseNode>>
          {
              { "additional_owners", n => { AdditionalOwners = n.GetCollectionOfPrimitiveValues<string>()?.AsList(); } },
              { "media", n => { Media = n.GetObjectValue<MediaUploadRequestOneShot.MediaUploadRequestOneShot_media>(MediaUploadRequestOneShot.MediaUploadRequestOneShot_media.CreateFromDiscriminatorValue); } },
              { "media_category", n => { MediaCategory = n.GetEnumValue<MediaCategoryOneShot>(); } },
              { "media_type", n => { MediaType = n.GetEnumValue<MediaUploadRequestOneShot_media_type>(); } },
              { "shared", n => { Shared = n.GetBoolValue(); } },
          };
  }
  
  public virtual void Serialize(ISerializationWriter writer)
  {
    if (ReferenceEquals(writer, null)) throw new ArgumentNullException(nameof(writer));
    writer.WriteCollectionOfPrimitiveValues<string>("additional_owners", AdditionalOwners);
    writer.WriteObjectValue<MediaUploadRequestOneShot.MediaUploadRequestOneShot_media>("media", Media);
    writer.WriteEnumValue<MediaCategoryOneShot>("media_category", MediaCategory);
    writer.WriteEnumValue<MediaUploadRequestOneShot_media_type>("media_type", MediaType);
    writer.WriteBoolValue("shared", Shared);
  }
}
```

I modified the property `Media` to be of type `byte[]`. One line in `GetFieldDeserializers` and `Serialize` had
to be changed, too:

```c#
public partial class MediaUploadRequestOneShot : IParsable
{
  ...
  public byte[] Media { get; set; }
  ...
  
  public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers()
  {
    return new Dictionary<string, Action<IParseNode>>
          {
                ...
              { "media", n => { Media = n.GetByteArrayValue(); } },
              ....
  
          };
  }
  
  public virtual void Serialize(ISerializationWriter writer)
  {
    ...
    writer.WriteByteArrayValue("media", Media);
    ...
  }
```

Now, upload works with this piece of code:

```c#
byte[] data = ...;
MediaUploadRequestOneShot mediaUpload = new MediaUploadRequestOneShot();
mediaUpload.MediaCategory = MediaCategoryOneShot.Tweet_image;
mediaUpload.Media = data;

await xClient.Two.Media.Upload.PostAsync(mediaUpload);
```

Looking at the request, we see that Kiota uploads a base64 string.

Note: this issue does not happen when using the "chunked" media upload (initialize/append/finalize): https://docs.x.com/x-api/media/initialize-media-upload. 
So, there is something in the OpenAPI description that Kiota cannot handle

# What about other OpenAPI libraries?

Before switching to Kiota, I tested [NSwag](https://github.com/RicoSuter/NSwag/), but this fails miserably: it creates 
a REST client that does not even compile. And digging deeper reveals that it does not support the "oneOf" declaration which is heavily used by X,
NSwag just picks the first class: https://github.com/RicoSuter/NSwag/issues/3738
