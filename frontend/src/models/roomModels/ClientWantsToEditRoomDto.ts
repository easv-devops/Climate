import {BaseDto} from "../baseDto";



export class ClientWantsToEditRoomDto extends BaseDto<ClientWantsToEditRoomDto> {
  RoomToEdit!: RoomToEdit;
}

export class RoomToEdit {
  Id!: number;
  RoomName!: string;
}
