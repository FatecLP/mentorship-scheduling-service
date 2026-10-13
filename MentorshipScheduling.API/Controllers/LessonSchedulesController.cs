using Andras.Result.Errors;
using MentorshipScheduling.Application.DTOs;
using MentorshipScheduling.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MentorshipScheduling.API.Controllers;

[ApiController]
[Route("api/lesson-schedules")]
public sealed class LessonSchedulesController : ControllerBase
{
    private readonly ILessonScheduleService _service;

    public LessonSchedulesController(ILessonScheduleService service)
    {
        this._service = service;
    }

    /// <summary>Lista os agendamentos do aluno ou professor informado.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<LessonScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<LessonScheduleResponse>>> GetAll(
        [FromQuery] long? mentorshipId,
        [FromQuery] long? studentId,
        [FromQuery] long? teacherId)
    {
        var result = await this._service.GetAllAsync(
            mentorshipId,
            studentId,
            teacherId);

        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error!);
    }

    /// <summary>Consulta um agendamento pertencente ao aluno ou professor informado.</summary>
    [HttpGet("{id:long}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(LessonScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonScheduleResponse>> GetById(
        long id,
        [FromQuery] long? studentId,
        [FromQuery] long? teacherId)
    {
        var result = await this._service.GetByIdAsync(
            id,
            studentId,
            teacherId);

        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error!);
    }

    /// <summary>Solicita uma aula para uma mentoria aceita.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(LessonScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LessonScheduleResponse>> Create(
        CreateLessonScheduleRequest request)
    {
        var result = await this._service.CreateAsync(request);
        if (result.IsFailure)
        {
            return ToProblem(result.Error!);
        }

        var schedule = result.Value!;
        return CreatedAtAction(
            nameof(GetById),
            new { id = schedule.Id, studentId = request.StudentId },
            schedule);
    }

    /// <summary>Altera a data e o formato de uma solicitação pendente do aluno informado.</summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(LessonScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LessonScheduleResponse>> Update(
        long id,
        UpdateLessonScheduleRequest request)
    {
        var result = await this._service.UpdateAsync(id, request);
        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error!);
    }

    /// <summary>Aprova uma solicitação pendente usando o ID do professor da mentoria.</summary>
    [HttpPost("{id:long}/approve")]
    [ProducesResponseType(typeof(LessonScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LessonScheduleResponse>> Approve(
        long id,
        LessonScheduleActorRequest request)
    {
        var result = await this._service.ApproveAsync(
            id,
            request.TeacherId.GetValueOrDefault());

        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error!);
    }

    /// <summary>Rejeita uma solicitação pendente usando o ID do professor da mentoria.</summary>
    [HttpPost("{id:long}/reject")]
    [ProducesResponseType(typeof(LessonScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LessonScheduleResponse>> Reject(
        long id,
        LessonScheduleActorRequest request)
    {
        var result = await this._service.RejectAsync(
            id,
            request.TeacherId.GetValueOrDefault());

        return result.IsSuccess
            ? Ok(result.Value)
            : ToProblem(result.Error!);
    }

    /// <summary>Cancela uma solicitação do aluno ou professor vinculado à mentoria.</summary>
    [HttpPost("{id:long}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cancel(
        long id,
        LessonScheduleActorRequest request)
    {
        var result = await this._service.CancelAsync(
            id,
            request.StudentId,
            request.TeacherId);

        return result.IsSuccess
            ? NoContent()
            : ToProblem(result.Error!);
    }

    private ObjectResult ToProblem(Error error)
    {
        var status = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = error.Message,
            Detail = error.Code,
            Instance = HttpContext.Request.Path
        };
        problemDetails.Extensions["errors"] = error.Details;
        problemDetails.Extensions["code"] = error.Code;

        return StatusCode(status, problemDetails);
    }
}
