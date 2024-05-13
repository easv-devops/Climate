import { check } from 'k6';
import ws from 'k6/ws';
import { sleep } from 'k6';

export let options = {
    stages: [
        { duration: '10s', target: 50 },
        { duration: '1m40s', target: 50 }, // En meget kort load test. Når stadig 2K+ Client Events
        { duration: '10s', target: 0 },
    ],
    thresholds: {
        'checks': ['rate == 1'], // 100% for checket: resp.eventType === 'ServerAuthenticatesUser'
    }
};

export default function () {
    const res = ws.connect('ws://74.234.8.67:5002', {}, function (socket) {
        socket.on('open', function open() {
            // Fjern outcommenting ved fejlsøgning
            // console.log('connected');
            const message = {
                eventType: 'ClientWantsToAuthenticate',
                email: 'user@example.com',
                password: '12345678'
            };
            socket.send(JSON.stringify(message));
        });
        socket.on('message', function incoming(data) {
            // Fjern outcommenting ved fejlsøgning
            // console.log('Message received:', data);
            const response = JSON.parse(data);
            // Expecting first response from login to be ServerReturnsAllRooms 
            // (happens as ClientWantsToAuthenticate.cs calls ServerWantsToInitUser.cs)
            check(response, { 'EventType is ServerReturnsAllRooms': (resp) => resp.eventType === 'ServerReturnsAllRooms' });
            socket.close();
        });
        socket.on('close', function () {
            // Fjern outcommenting ved fejlsøgning
            // console.log('disconnected');
            sleep(1);
        });
    });

    if (!res) {
        console.error('WebSocket connection failed');
    }
}
