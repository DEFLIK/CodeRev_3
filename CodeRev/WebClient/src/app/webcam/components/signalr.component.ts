import {Component, ElementRef, OnDestroy, OnInit, ViewChild} from "@angular/core";
import {SignalrService} from "../services/signalr.service";
import {RtcService} from "../services/rtc.service";
import {PeerData, SignalInfo, UserInfo, UserVideo} from "../models/peerData.interface";
import {Subscription} from "rxjs";
import {UrlRoutes} from "../../global-services/request/models/url-routes";
import {userInfo} from "os";
import {ICodeRecord} from "../../code-editor/models/codeRecord";

@Component({
  selector: 'app-webcam',
  templateUrl: './signalr.component.html'
})
export class SignalrComponent implements OnInit, OnDestroy {

  @ViewChild('otherVideoPlayer') otherVideoPlayer!: ElementRef;
  @ViewChild('ourVideoPlayer') ourVideoPlayer!: ElementRef;

  public subscriptions = new Subscription();
  public currentUserName!: string;
  public roomName!: string;
  public message!: string;
  public dataString!: string;
  public userVideo!: string;
  public otherUser!: UserInfo | null;
  private stream!: MediaStream;
  private mediaRecorder!: MediaRecorder;
  private mediaChunks!: Blob[];

  constructor(private rtcService: RtcService, private signalR: SignalrService) {
  }

  public mediaError = (): void => {
    console.error(`Can't get user media`);
  };

  ngOnInit() {

    this.subscriptions.add(this.signalR.newPeer$.subscribe((user: UserInfo) => {
      this.rtcService.newUser(user);
      this.signalR.sayHello(this.currentUserName, this.roomName, user.connectionId);
      console.log(user.userName);
      if (!this.otherUser) {
        this.otherUser = user;
      }
    }));

    this.subscriptions.add(this.signalR.helloAnswer$.subscribe((user: UserInfo) => {
      if (!this.otherUser) {
        this.otherUser = user;
      }
      this.rtcService.newUser(user);
      this.onUserSelected(user);
    }));

    this.subscriptions.add(this.signalR.disconnectedPeer$.subscribe((user: UserInfo) => {
      this.otherUser = null;
      this.otherVideoPlayer.nativeElement = null;
      this.rtcService.disconnectedUser(user);
    }));

    this.subscriptions.add(this.signalR.signal$.subscribe((signalData: SignalInfo) => {
      this.rtcService.signalPeer(signalData.user, signalData.signal, this.stream);
    }));

    this.subscriptions.add(this.rtcService.onSignalToSend$.subscribe((data: PeerData) => {
      this.signalR.sendSignalToUser(data.data, data.id);
    }));

    this.subscriptions.add(this.rtcService.onStream$.subscribe((data: PeerData) => {
      this.userVideo = data.id;
      this.otherVideoPlayer.nativeElement.srcObject = data.data;
      this.otherVideoPlayer.nativeElement.load();
      this.otherVideoPlayer.nativeElement.play();
    }));
  }

  public onUserSelected(userInfo: UserInfo) {
    const peer = this.rtcService.createPeer(this.stream, userInfo.connectionId, true);
    this.rtcService.currentPeer = peer;
  }

  public async sendData() {
    if (this.otherUser) {
      this.signalR.sendData(this.otherUser.connectionId, this.message);
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

  ngOnDestroy() {
    this.subscriptions.unsubscribe();
  }

  private async turnOnWebCamera() {
    this.stream = await navigator.mediaDevices.getUserMedia({video: true, audio: true});
    this.ourVideoPlayer.nativeElement.srcObject = this.stream;
    this.ourVideoPlayer.nativeElement.playsInline = true;
    this.ourVideoPlayer.nativeElement.muted = true;
    this.ourVideoPlayer.nativeElement.load();
    this.ourVideoPlayer.nativeElement.play();

    this.mediaRecorder = new MediaRecorder(this.stream);

    this.mediaRecorder.start();
    this.mediaChunks = [];
    this.mediaRecorder.addEventListener("dataavailable", this.pushMedia);
  }

  private pushMedia(event: BlobEvent){
    console.log(event.data.arrayBuffer());
    try {
      this.mediaChunks.push(event.data);
    }catch (e) {
      console.log(event.data.arrayBuffer());
    }
  }

  public async stopRecorder(){
    this.mediaRecorder.addEventListener("stop", this.stop);
    this.mediaRecorder.stop();
  }

  private async stop(){
    const videoBlob = new Blob(this.mediaChunks, {
      type: "video/webm"
    });

    console.log(this.mediaChunks);

    let fd = new FormData();
    fd.append('video', videoBlob);
    console.log(fd);
    await SignalrComponent.sendMedia(new UserVideo(videoBlob));
    this.mediaChunks = [];
  }

  private static async sendMedia(form: UserVideo) {
    let promise = await fetch(`${UrlRoutes.tracker}/api/v1.0/tracker/save-video`, {
      method: 'POST',
      body: JSON.stringify(form),
    });
    if (promise.ok) {
      console.log(promise);
    }
  }
}
