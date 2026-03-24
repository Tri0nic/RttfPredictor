using System.Data;
using FluentValidation;
using ReactApp1.Server.DTO;

namespace ReactApp1.Server.Validators
{
    public class GetPlayersValidator : AbstractValidator<GetPlayersRequest>
        {
            public GetPlayersValidator()
            {
            RuleFor(x => x.Id)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Id не может быть меньше либо равен нулю");
            RuleFor(x => x.Rating)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Рейтинг не может быть отрицательным");
        }
    }
}
