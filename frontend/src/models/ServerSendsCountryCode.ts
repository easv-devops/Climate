import {BaseDto} from "./baseDto";



export class ServerSendsCountryCodeDto extends BaseDto<ServerSendsCountryCodeDto> {
  CountryCode!: number[];
}
