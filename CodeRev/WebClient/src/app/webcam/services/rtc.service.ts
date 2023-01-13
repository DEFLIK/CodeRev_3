/* eslint-disable object-curly-spacing */
/* eslint-disable quotes */
/* eslint-disable @typescript-eslint/array-type */
/* eslint-disable @typescript-eslint/naming-convention */
/* eslint-disable @typescript-eslint/member-ordering */
import {Injectable} from "@angular/core";
import {BehaviorSubject, Observable, Subject} from "rxjs";
import {PeerData, UserInfo} from "../models/peerData.interface";
import {Instance} from "simple-peer";

declare var SimplePeer: any;

@Injectable({
    providedIn: 'root'
})
export class RtcService{

    private users: BehaviorSubject<Array<UserInfo>>;
    public users$: Observable<Array<UserInfo>>;

    private onSignalToSend = new Subject<PeerData>();
    public onSignalToSend$ = this.onSignalToSend.asObservable();

    private onStream = new Subject<PeerData>();
    public onStream$ = this.onStream.asObservable();

    private onConnect = new Subject<PeerData>();
    public onConnect$ = this.onConnect.asObservable();

    private onData = new Subject<PeerData>();
    public onData$ = this.onData.asObservable();

    public currentPeer!: Instance | null;

    constructor() {
        this.users = new BehaviorSubject<Array<UserInfo>>([]);
        this.users$ = this.users.asObservable();
    }

    public newUser(user: UserInfo): void {
        this.users.next([...this.users.getValue(), user]);
    }

    public disconnectedUser(user: UserInfo): void {
        const filteredUsers = this.users.getValue().filter(x => x.connectionId === user.connectionId);
        this.users.next(filteredUsers);
        if (this.currentPeer) {
            this.currentPeer.destroy();
            this.currentPeer = null;
        }
    }

    public disconnect(): void {
        this.currentPeer?.destroy();
        this.currentPeer = null;
    }

    public createPeer(stream:MediaStream, userId: string, initiator: boolean): Instance {
        const peer = new SimplePeer({ initiator, stream });

        peer.on('signal', (data: any) => {
            console.log('on signal', data);
            if (data.renegotiate || data.transceiverRequest) {
                return;
            };
            const stringData = JSON.stringify(data);
            this.onSignalToSend.next({ id: userId, data: stringData });
        });

        peer.on('stream', (data: any) => {
            console.log('on stream', data);
            this.onStream.next({ id: userId, data });
        });

        peer.on('connect', () => {
            console.log('on connect');
            this.onConnect.next({ id: userId, data: null });
        });

        peer.on('data', (data: any) => {
            console.log('on data', data);
            this.onData.next({ id: userId, data });
        });

        peer.on('close', ()=>{
            console.log('close');
            this.currentPeer = null;
        });

        return peer;
    }

    public signalPeer(userId: string, signal: string, stream: any): void {
        const signalObject = JSON.parse(signal);
      
        if (this.currentPeer) {
            this.currentPeer.signal(signalObject);
        } else {
            this.currentPeer = this.createPeer(stream, userId, false);
            this.currentPeer.signal(signalObject);
        }
    }
    //
    // public sendMessage(message: string) {
    //   this.currentPeer.send(message);
    // }
}
