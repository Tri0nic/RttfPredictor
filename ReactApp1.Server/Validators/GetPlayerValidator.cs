using System.Data;
using FluentValidation;
using ReactApp1.Server.DTO;

namespace ReactApp1.Server.Validators
{
    public class GetPlayerValidator : AbstractValidator<GetPlayerRequest>
    {
        public GetPlayerValidator() 
        {
            RuleFor(x => x.Rating)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Рейтинг не может быть отрицательным");
        }
    }
}
