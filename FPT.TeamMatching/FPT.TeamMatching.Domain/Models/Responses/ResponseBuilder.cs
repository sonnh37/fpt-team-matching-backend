namespace FPT.TeamMatching.Domain.Models.Responses;

// Ngựa khi gán WithData, WithStatus, WithMessage trả về cho rõ thôi 
// Ae có thể dùng new BusinessResult hoặc new ResponseBuilder
public class ResponseBuilder 
{
    protected string _message;

    protected int _status;
    
    protected object? _data;
    
    // Các phương thức hỗ trợ method chaining
    public ResponseBuilder WithStatus(int status)
    {
        _status = status;
        return this;
    }

    public ResponseBuilder WithMessage(string message)
    {
        _message = message;
        return this;
    }
    
    public ResponseBuilder WithData(object data)
    {
        _data = data;
        return this;
    }

    private BusinessResult Build()
    {
        return _data == null ? new BusinessResult(_status, _message) : new BusinessResult(_status, _message, _data);
    }
    
    public static implicit operator BusinessResult(ResponseBuilder builder)
    {
        return builder.Build();
    }
}

