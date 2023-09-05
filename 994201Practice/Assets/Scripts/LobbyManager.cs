using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using UnityEngine;
using UnityEngine.UI;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks {
    private string gameVersion = "1"; // 게임 버전

    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    public Button joinButton; // 룸 접속 버튼

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start() {
        // 접속에 필요한 정보 설정
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinButton.interactable = false;
        connectionInfoText.text = "Connect to master server ...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() {
        joinButton.interactable = true;
        connectionInfoText.text = "Online: Connected to master server succeed";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause) {
        joinButton.interactable = false;
        connectionInfoText.text = string.Format("{0}\n{1}", 
            "Offline: Disconnected to master server", "Retry connect now ...");

        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸 접속 시도
    public void Connect()
    {
        joinButton.interactable = false;

        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "Connect to Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = string.Format("{0}\n{1}", 
                "Offline: Disconnected to master server", "Retry connect now ...");
        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message) {
        connectionInfoText.text = "Nothing to empty room, Create new room ...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() {
        connectionInfoText.text = "Successes join room";
        PhotonNetwork.LoadLevel("PlayScene");
    }
}