public record HttpFetchResult(
    bool IsSuccess,
    int StatusCode,
    string Content);