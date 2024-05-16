import {BaseDto} from "./baseDto";
import {CountryCode} from "./Entities";



export class ServerSendsCountryCodesDto extends BaseDto<ServerSendsCountryCodesDto> {
  CountryCode!: CountryCode[];
}
