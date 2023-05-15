/* eslint-disable indent */
/* eslint-disable @typescript-eslint/member-ordering */
/* eslint-disable @typescript-eslint/naming-convention */
/* eslint-disable object-curly-spacing */
/* eslint-disable quotes */
import {Injectable} from "@angular/core";
import * as signalR from "@aspnet/signalr";
import {Subject} from "rxjs";
import { UrlRoutes } from "../../global-services/request/models/url-routes";


@Injectable({
  providedIn: 'root'
})
export class NotificationSignalrService{

  private hubConnection!: signalR.HubConnection;

  public notification = new Subject<string>();

  constructor() {  }

  public async startConnection(roomName: string): Promise<void> {
    console.log('HUB CONNECTED -----------------------', roomName);

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${UrlRoutes.tracker}/notificationHub`)
      .build();

    await this.hubConnection.start();
    console.log('Notification Connection started');

    this.hubConnection.on('NewUserArrived', (data) =>{
      console.log('New User Arrived', data);
    });

    this.hubConnection.on('SendNotification', (data) => {
      this.notification.next(data);
      console.log("Send Notification");
    });

    this.hubConnection.invoke('NewUser', "wee", roomName);
  }
}
