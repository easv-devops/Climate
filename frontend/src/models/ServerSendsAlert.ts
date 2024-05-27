import {BaseDto} from "./baseDto";
import {AlertDto} from "./Entities";

export class ServerSendsAlert extends BaseDto<ServerSendsAlert> {
  Alert!: AlertDto;
}
