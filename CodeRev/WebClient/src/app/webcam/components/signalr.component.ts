/* eslint-disable object-curly-spacing */
/* eslint-disable quotes */
/* eslint-disable @typescript-eslint/member-ordering */
import { Component, ElementRef, OnDestroy, OnInit, ViewChild} from '@angular/core';
import { SignalrService } from '../services/signalr.service';
import { RtcService } from '../services/rtc.service';
import { PeerData, SignalInfo, UserInfo, UserVideo } from '../models/peerData.interface';
import { Subscription } from 'rxjs';
import { UrlRoutes } from '../../global-services/request/models/url-routes';


@Component({
    selector: 'app-webcam',
    templateUrl: './signalr.component.html'
})
export class SignalrComponent implements OnInit, OnDestroy {

  @ViewChild('otherVideoPlayer') public otherVideoPlayer!: ElementRef;
  @ViewChild('ourVideoPlayer') public ourVideoPlayer!: ElementRef;

  public subscriptions = new Subscription();
  public currentUserName!: string;
  public roomName!: string;
  public message!: string;
  public dataString!: string;
  public userVideo!: string;
  public otherUser!: UserInfo | null;
  private _stream!: MediaStream;
  private _mediaRecorder!: MediaRecorder;
  private _mediaChunks!: Blob[];

  constructor(private _rtcService: RtcService, private _signalR: SignalrService) {
  }

  public mediaError = (): void => {
      console.error(`Can't get user media`);
  };

  public ngOnInit(): void {
      this.subscriptions.add(this._signalR.newPeer$.subscribe((user: UserInfo) => {
          this._rtcService.newUser(user);
          this._signalR.sayHello(this.currentUserName, this.roomName, user.connectionId);
          console.log(user.userName);
          this.otherUser = user;
      }));

      this.subscriptions.add(this._signalR.helloAnswer$.subscribe((user: UserInfo) => {
          this.otherUser = user;
          this._rtcService.newUser(user);
          this.onUserSelected(user);
      }));

      this.subscriptions.add(this._signalR.disconnectedPeer$.subscribe((user: UserInfo) => {
          this.otherUser = null;
          this.otherVideoPlayer.nativeElement = null;
          this._rtcService.disconnectedUser(user);
      }));

      this.subscriptions.add(this._signalR.signal$.subscribe((signalData: SignalInfo) => {
          this._rtcService.signalPeer(signalData.user, signalData.signal, this._stream);
      }));

      this.subscriptions.add(this._rtcService.onSignalToSend$.subscribe((data: PeerData) => {
          this._signalR.sendSignalToUser(data.data, data.id);
      }));

      this.subscriptions.add(this._rtcService.onStream$.subscribe((data: PeerData) => {
          this.userVideo = data.id;
          this.otherVideoPlayer.nativeElement.srcObject = data.data;
          this.otherVideoPlayer.nativeElement.load();
          this.otherVideoPlayer.nativeElement.play();
      }));
  }

  public onUserSelected(userInfo: UserInfo): void {
      const peer = this._rtcService.createPeer(this._stream, userInfo.connectionId, true);
      this._rtcService.currentPeer = peer;
  }

  public async sendData(): Promise<void> {
      if (this.otherUser) {
          this._signalR.sendData(this.otherUser.connectionId, this.message);
      }
  }

  public async saveUsername(): Promise<void> {
      try {
          await this.turnOnWebCamera();
          await this._signalR.startConnection(this.currentUserName, this.roomName);
      } catch (error) {
          console.error(`Can't join room, error ${error}`);
      }
  }

  public async disconnect(): Promise<void> {
      await this._signalR.disconnect();
      this._rtcService.disconnect();
  }

  public ngOnDestroy(): void {
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
