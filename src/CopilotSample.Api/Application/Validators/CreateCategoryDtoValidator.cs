using FluentValidation;
using CopilotSample.Api.Application.DTOs;

namespace CopilotSample.Api.Application.Validators
{
    public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("カテゴリ名は必須です。")
                .MaximumLength(50).WithMessage("カテゴリ名は50文字以内で入力してください。");
        }
    }
}
