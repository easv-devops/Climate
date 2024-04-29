export interface Room{
  roomname: string;
  roomId: number;
  image: string;
  currentTemperature?: number;
  currentHumidity?: number;
}
export interface RoomRating{
  room: Room;
  currentTemperature: string;
  currentHumidity: string;
  score: number;
}
