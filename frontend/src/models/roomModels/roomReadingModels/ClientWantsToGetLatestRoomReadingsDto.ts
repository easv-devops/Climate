import {BaseDto} from "../../baseDto";

export class ClientWantsToGetLatestRoomReadingsDto extends BaseDto<ClientWantsToGetLatestRoomReadingsDto> {
  RoomId!: number;
}
