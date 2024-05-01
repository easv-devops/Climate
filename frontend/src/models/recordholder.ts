
export class RecordHolder<T> {
  records: { [key: number]: T };

  constructor() {
    this.records = {};
  }

  addRecord(key: number, values: T) {
    this.records[key] = values;
  }

  getRecord(key: number): T | undefined {
    return this.records[key];
  }

  getAllRecords(): { [key: number]: T } {
    return this.records;
  }
}
