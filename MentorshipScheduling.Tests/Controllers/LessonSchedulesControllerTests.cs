using Andras.Result;
using Andras.Result.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MentorshipScheduling.API.Controllers;
using MentorshipScheduling.Application.DTOs;
using MentorshipScheduling.Application.Services;
using MentorshipScheduling.Domain.Enums;
using MentorshipScheduling.Tests.MockData;
using Moq;

namespace MentorshipScheduling.Tests.Controllers;

public sealed class LessonSchedulesControllerTests
{
    private readonly Mock<ILessonScheduleService> _mockService;
    private readonly LessonSchedulesController _controller;

    public LessonSchedulesControllerTests()
    {
        this._mockService = new Mock<ILessonScheduleService>();
        this._controller = new LessonSchedulesController(this._mockService.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithSchedules()
    {
        this._mockService
            .Setup(service => service.GetAllAsync(null, 23, null))
            .ReturnsAsync(Result<IReadOnlyList<LessonScheduleResponse>>.Success(
                LessonScheduleMockData.GetAll()));

        var result = await this._controller.GetAll(null, 23, null);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOkWithSchedule()
    {
        this._mockService
            .Setup(service => service.GetByIdAsync(7, 23, null))
            .ReturnsAsync(Result<LessonScheduleResponse>.Success(
                LessonScheduleMockData.GetById(7)));

        var result = await this._controller.GetById(7, 23, null);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedSchedule()
    {
        var request = LessonScheduleMockData.CreateRequest();
        this._mockService
            .Setup(service => service.CreateAsync(request))
            .ReturnsAsync(Result<LessonScheduleResponse>.Success(
                LessonScheduleMockData.GetById(7)));

        var result = await this._controller.Create(request);

        Assert.IsType<CreatedAtActionResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOkWithUpdatedSchedule()
    {
        var request = LessonScheduleMockData.UpdateRequest();
        this._mockService
            .Setup(service => service.UpdateAsync(7, request))
            .ReturnsAsync(Result<LessonScheduleResponse>.Success(
                LessonScheduleMockData.GetById(7)));

        var result = await this._controller.Update(7, request);

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Approve_ReturnsOkWithApprovedSchedule()
    {
        this._mockService
            .Setup(service => service.ApproveAsync(7, 34))
            .ReturnsAsync(Result<LessonScheduleResponse>.Success(
                LessonScheduleMockData.GetById(7, LessonScheduleStatus.Approved)));

        var result = await this._controller.Approve(
            7,
            new LessonScheduleActorRequest { TeacherId = 34 });

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Reject_ReturnsOkWithRejectedSchedule()
    {
        this._mockService
            .Setup(service => service.RejectAsync(7, 34))
            .ReturnsAsync(Result<LessonScheduleResponse>.Success(
                LessonScheduleMockData.GetById(7, LessonScheduleStatus.Rejected)));

        var result = await this._controller.Reject(
            7,
            new LessonScheduleActorRequest { TeacherId = 34 });

        Assert.IsType<OkObjectResult>(result.Result);
    }

    [Fact]
    public async Task Cancel_ReturnsNoContent()
    {
        this._mockService
            .Setup(service => service.CancelAsync(7, 23, null))
            .ReturnsAsync(Result<LessonScheduleResponse>.Success(
                LessonScheduleMockData.GetById(7, LessonScheduleStatus.Cancelled)));

        var result = await this._controller.Cancel(
            7,
            new LessonScheduleActorRequest { StudentId = 23 });

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsConflictWhenTimeIsUnavailable()
    {
        this._controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        var request = LessonScheduleMockData.CreateRequest();
        this._mockService
            .Setup(service => service.CreateAsync(request))
            .ReturnsAsync(Result<LessonScheduleResponse>.Failure(
                Error.Conflict("schedule.time_conflict", "O horário não está disponível.")));

        var result = await this._controller.Create(request);

        Assert.Equal(
            StatusCodes.Status409Conflict,
            Assert.IsType<ObjectResult>(result.Result).StatusCode);
    }
}
