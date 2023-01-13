/* eslint-disable object-curly-spacing */
/* eslint-disable quotes */
/* eslint-disable @typescript-eslint/member-ordering */
import { Component, ElementRef, Input, OnDestroy, OnInit, Output, ViewChild} from '@angular/core';
import { SignalrService } from '../services/signalr.service';
import { RtcService } from '../services/rtc.service';
import { PeerData, SignalInfo, UserInfo, UserVideo } from '../models/peerData.interface';
import { Observable, of, Subject, Subscription } from 'rxjs';
import { UrlRoutes } from '../../global-services/request/models/url-routes';


@Component({
    selector: 'app-webcam',
    templateUrl: './signalr.component.html',
    styleUrls: ['./signalr.component.less']
})
export class SignalrComponent implements OnInit, OnDestroy {

  @ViewChild('otherVideoPlayer') public otherVideoPlayer!: ElementRef;
  @ViewChild('ourVideoPlayer') public ourVideoPlayer!: ElementRef;
  public subscriptions = new Subscription();
  @Input()
  public currentUserName!: string;
  @Input()
  public roomName!: string;
  @Output()
  public dataRecieved$!: Observable<PeerData>;
  public message!: string;
  public dataString!: string;
  public userVideo!: string;
  public otherUser!: UserInfo | null;
  private _stream!: MediaStream;
  private _mediaRecorder!: MediaRecorder;
  private _mediaChunks!: Blob[];

  constructor(public rtcService: RtcService, public signalR: SignalrService) {
      this.dataRecieved$ = rtcService.onData$;
  }

  public mediaError = (): void => {
      console.error(`Can't get user media`);
  };

  public async ngOnInit(): Promise<void> {
      this.subscriptions.add(this.signalR.newPeer$.subscribe((user: UserInfo) => {
          this.rtcService.newUser(user);
          this.signalR.sayHello(this.currentUserName, this.roomName, user.connectionId);
          console.log(user.userName);
          this.otherUser = user;
      }));

      this.subscriptions.add(this.signalR.helloAnswer$.subscribe((user: UserInfo) => {
          this.otherUser = user;
          this.rtcService.newUser(user);
          this.onUserSelected(user);
      }));

      this.subscriptions.add(this.signalR.disconnectedPeer$.subscribe((user: UserInfo) => {
          this.otherUser = null;
          this.otherVideoPlayer.nativeElement = null;
          this.rtcService.disconnectedUser(user);
      }));

      this.subscriptions.add(this.signalR.signal$.subscribe((signalData: SignalInfo) => {
          this.rtcService.signalPeer(signalData.user, signalData.signal, this._stream);
      }));

      this.subscriptions.add(this.rtcService.onSignalToSend$.subscribe((data: PeerData) => {
          this.signalR.sendSignalToUser(data.data, data.id);
      }));

      this.subscriptions.add(this.rtcService.onStream$.subscribe((data: PeerData) => {
		  console.log(data);
		  
          this.userVideo = data.id;
          this.otherVideoPlayer.nativeElement.srcObject = data.data;
          this.otherVideoPlayer.nativeElement.load();
          this.otherVideoPlayer.nativeElement.play();
      }));

	  await this.saveUsername();
  }

  public onUserSelected(userInfo: UserInfo): void {
      const peer = this.rtcService.createPeer(this._stream, userInfo.connectionId, true);
      this.rtcService.currentPeer = peer;
  }

  public async sendData(data: string): Promise<void> {
      if (this.otherUser) {
          this.signalR.sendData(this.otherUser.connectionId, data);
      }
  }

  public async saveUsername(): Promise<void> {
      try {
          await this.turnOnWebCamera();
          await this.signalR.startConnection(this.currentUserName, this.roomName);
      } catch (error) {
          console.error(`Can't join room, error ${error}`);
      }
  }

  public async disconnect(): Promise<void> {
      await this.signalR.disconnect();
      this.rtcService.disconnect();
  }

  public ngOnDestroy(): void {
	  this.disconnect();
      this.subscriptions.unsubscribe();
  }

  public stopRecorder(): void {
      this._mediaRecorder.addEventListener("stop", this.stop);
      this._mediaRecorder.stop();
  }

  private async turnOnWebCamera(): Promise<void> {
      this._stream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});
      this.ourVideoPlayer.nativeElement.srcObject = this._stream;
      this.ourVideoPlayer.nativeElement.playsInline = true;
      this.ourVideoPlayer.nativeElement.muted = true;
      this.ourVideoPlayer.nativeElement.load();
      this.ourVideoPlayer.nativeElement.play();

      this._mediaRecorder = new MediaRecorder(this._stream);

      this._mediaRecorder.start();
      this._mediaChunks = [];
      this._mediaRecorder.addEventListener('dataavailable', this.pushMedia);
  }

  private pushMedia(event: BlobEvent): void {
      console.log(event.data.arrayBuffer());
      try {
          this._mediaChunks.push(event.data);
      }catch (e) {
          console.log(event.data.arrayBuffer());
      }
  }

  private async stop(): Promise<void> {
      const videoBlob = new Blob(this._mediaChunks, {
          type: "video/webm"
      });

      console.log(this._mediaChunks);

      const fd = new FormData();
      fd.append('video', videoBlob);
      console.log(fd);
      await SignalrComponent.sendMedia(new UserVideo(videoBlob));
      this._mediaChunks = [];
  }

  private static async sendMedia(form: UserVideo): Promise<void> {
      const promise = await fetch(`${UrlRoutes.tracker}/api/v1.0/tracker/save-video`, {
          method: 'POST',
          body: JSON.stringify(form),
      });
      if (promise.ok) {
          console.log(promise);
      }
  }
}
