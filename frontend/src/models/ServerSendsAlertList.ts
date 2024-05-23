import {BaseDto} from "./baseDto";
import {AlertDto} from "./Entities";

export class ServerSendsAlertList extends BaseDto<ServerSendsAlertList> {
  Alerts?: AlertDto[];
}
