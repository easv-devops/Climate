import {BaseDto} from "../baseDto";



export class ClientWantsToCreateRoomDto extends BaseDto<ClientWantsToCreateRoomDto> {
  RoomToCreate!: CreateRoomDto;
}

export class CreateRoomDto {
  RoomName!: string;
}

