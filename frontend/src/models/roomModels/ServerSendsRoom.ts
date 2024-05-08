import {BaseDto} from "../baseDto";


export class ServerSendsRoom extends BaseDto<ServerSendsRoom>  {
  Room!: RoomWithId;
}

export class RoomWithId {
  Id!: number;
  RoomName!: string;
}
