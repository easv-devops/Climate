﻿using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Security.Authentication;
using api.ClientEventFilters;
using api.helpers;
using api.serverEventModels;
using api.WebSocket;
using Fleck;
using lib;
using service.services;

namespace api.clientEventHandlers;

public class ClientWantsToDeleteDeviceDto : BaseDto
{ 
    [Required(ErrorMessage = "Device Id is required.")]
    [Range(0, int.MaxValue, ErrorMessage = "Device Id is not a valid number")]
    public int Id { get; set; }
}

[RequireAuthentication]
[ValidateDataAnnotations]
public class ClientWantsToDeleteDevice : BaseEventHandler<ClientWantsToDeleteDeviceDto>
{
    private readonly DeviceService _deviceService;
    private readonly DeviceReadingsService _deviceReadingsService;
    private readonly DeviceSettingsService _deviceSettingsService;
    private readonly AlertService _alertService;

    public ClientWantsToDeleteDevice(
        DeviceService deviceService, 
        DeviceReadingsService deviceReadingsService, 
        DeviceSettingsService deviceSettingsService,
        AlertService alertService)
    {
        _deviceService = deviceService;
        _deviceReadingsService = deviceReadingsService;
        _deviceSettingsService = deviceSettingsService;
        _alertService = alertService;
    }
    
    
    public override Task Handle(ClientWantsToDeleteDeviceDto dto, IWebSocketConnection socket)
    {
        //checks if the user has permission before deleting
        var guid = socket.ConnectionInfo.Id;

        if (!StateService.UserHasDevice(guid, dto.Id))
        {
            throw new AuthenticationException("Only the owner of device #"+dto.Id+" has access to this information");
        }

        //removes device settings and range settings.
        if (!_deviceSettingsService.DeleteSettings(dto.Id) || !_deviceSettingsService.DeleteRangeSettings(dto.Id))
        {
            throw new SqlTypeException("Something went wrong when deleting device Settings #" + dto.Id);
        }
        
        // Removes alerts linked to device
        if (!_alertService.DeleteAlerts(dto.Id))
        {
            throw new SqlTypeException("Something went wrong when deleting alerts from device #" + dto.Id);
        }
        
        // Removes readings for the device and then the device itself
        if (!_deviceReadingsService.DeleteAllReadings(dto.Id) || !_deviceService.DeleteDevice(dto.Id))
        {
            throw new SqlTypeException("Something went wrong when deleting device #" + dto.Id);
        }
        
        //removes the device from stateService
        StateService.RemoveUserFromDevice(dto.Id, socket.ConnectionInfo.Id);
        
        //return the is deleted bool
        socket.SendDto(new ServerSendsDeviceDeletionStatus
        {
            IsDeleted = _deviceService.DeleteDevice(dto.Id),
            Id = dto.Id
        });
        
        return Task.CompletedTask;
    }
}
