export interface PeerData{
  id: string;
  data: any;
}

export interface UserInfo{
  userName: string;
  groupName: string;
  connectionId: string;
}

export interface SignalInfo{
  user: string;
  signal: any;
}

export interface ChatMessage{
  own: boolean;
  message: string;
}

export class UserVideo{
  video: Blob;

  constructor(video: Blob) {
    this.video = video;
  }
}
