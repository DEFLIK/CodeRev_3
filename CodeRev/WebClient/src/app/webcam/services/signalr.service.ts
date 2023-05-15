/* eslint-disable @typescript-eslint/member-ordering */
/* eslint-disable @typescript-eslint/naming-convention */
/* eslint-disable object-curly-spacing */
/* eslint-disable quotes */
import {Injectable} from "@angular/core";
import * as signalR from "@aspnet/signalr";
import {Subject} from "rxjs";
import {SignalInfo, UserInfo} from "../models/peerData.interface";
import { MeetPeerData } from "src/app/global-services/request/models/meet-peer-data";
import { UrlRoutes } from "../../global-services/request/models/url-routes";


@Injectable({
    providedIn: 'root'
})
export class SignalrService{

    private hubConnection!: signalR.HubConnection;

    private newPeer = new Subject<UserInfo>();
    public newPeer$ = this.newPeer.asObservable();

    private helloAnswer = new Subject<UserInfo>();
    public helloAnswer$ = this.helloAnswer.asObservable();

    private disconnectedPeer = new Subject<UserInfo>();
    public disconnectedPeer$ = this.disconnectedPeer.asObservable();

    private signal = new Subject<SignalInfo>();
    public signal$ = this.signal.asObservable();

    public data = new Subject<MeetPeerData>();
    public data$ = this.data.asObservable();

    constructor() {  }

    public async startConnection(currentUserName: string, roomName: string): Promise<void> {
        console.log('HUB CONNECTED -----------------------', roomName);

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${UrlRoutes.tracker}/signalrtc`)
            .build();

        await this.hubConnection.start();
        console.log('Connection started');

        this.hubConnection.on('NewUserArrived', (data) =>{
            this.newPeer.next(JSON.parse(data));
            console.log('New User Arrived', data);
        });

        this.hubConnection.on('UserSaidHello', (data) => {
            this.helloAnswer.next(JSON.parse(data));
            console.log('User Said Hello', data);
        });

        this.hubConnection.on('UserDisconnect', (data) => {
            try {
                this.disconnectedPeer.next(JSON.parse(data));
            }catch (e) {
            }
            console.log('User Disconnect', data);
        });

        this.hubConnection.on('SendSignal', (user, signal) => {
            this.signal.next({ user, signal });
            console.log('Send Signal', user);
        });

        this.hubConnection.on('SendData', (userName, data) =>{
            // this.data.next(JSON.parse(data))
            this.data.next(JSON.parse(data) as MeetPeerData);
        });

        this.hubConnection.invoke('NewUser', currentUserName, roomName);
    }

    public sendSignalToUser(signal: string, user: string): void {
        this.hubConnection.invoke('SendSignal', signal, user);
    }

    public sayHello(userName: string, roomName: string, user: string): void{
        this.hubConnection.invoke('HelloUser', userName, roomName, user);
    }

    public sendData(userConncetionId: string, data:string): void{
        this.hubConnection.invoke('SendData', userConncetionId, data);
    }

    public disconnect(): Promise<void> {
        console.log('HUB DISCONNECT -----------------------------');

        return this.hubConnection.stop();
    }
}
