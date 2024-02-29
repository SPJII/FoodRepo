var videos = ["video1.mp4", "video2.mp4", "video3.mp4", "video4.mp4"];
var currentVideoIndex = 0;
var currentVideo;
var isPlaying = true;
var pauseButtonText = "Pause";

function startTimer() {
    setInterval(changeVideo, 12000); // 12 seconds
}

function changeVideo() {
    if (isPlaying) {
        currentVideoIndex = (currentVideoIndex + 1) % videos.length;
        currentVideo = videos[currentVideoIndex];
        document.getElementById('video').src = currentVideo;
    }
}

var videos = ["video1.mp4", "video2.mp4", "video3.mp4", "video4.mp4"];
var currentVideoIndex = 0;
var currentVideo;
var isPlaying = true;
var pauseButtonText = "Pause";

function startTimer() {
    setInterval(changeVideo, 12000); // 12 seconds
}

function changeVideo() {
    if (isPlaying) {
        currentVideoIndex = (currentVideoIndex + 1) % videos.length;
        currentVideo = videos[currentVideoIndex];
        document.getElementById('video').src = currentVideo;
    }
}

function playVideo() {
    var video = document.getElementById('video');
    video.play();
}

function pauseVideo() {
    var video = document.getElementById('video');
    video.pause();
}

document.getElementById('mode-toggle').addEventListener('change', function () {
    document.body.classList.toggle('light-mode');
});

function redirectToWebsite(url) {
  var newTab = window.open(url, "_blank");
  newTab.focus();
}

