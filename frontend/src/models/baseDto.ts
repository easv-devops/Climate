export class BaseDto<T> {
  eventType: string;

  constructor(eventType: string, init?: Partial<T>) {
    this.eventType = eventType;
    Object.assign(this, init);
  }
}
