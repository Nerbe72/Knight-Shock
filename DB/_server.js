const express = require('express');
const bodyParser = require('body-parser');
const fs = require('fs');
const app = express();
const port = 3000;

app.use(bodyParser.json());

app.post('/login', (req, res) => {
  const { username, password } = req.body;
  
  // db.json 파일을 읽어 사용자 목록을 가져옵니다.
  fs.readFile('./db.json', 'utf8', (err, data) => {
    if (err) {
      console.error('파일 읽기 에러:', err);
      return res.status(500).json({ message: '서버 에러' });
    }
    
    const users = JSON.parse(data).users;
    // 입력받은 username, password와 일치하는 사용자가 있는지 확인합니다.
    const user = users.find(u => u.username === username && u.password === password);
    
    if (user) {
      // 로그인 성공: 토큰 발급 등 추가 작업을 진행할 수 있음
      return res.json({ 
        success: true, 
        message: '로그인 성공', 
        userId: user.id 
      });
    } else {
      // 로그인 실패: 401 Unauthorized 상태 코드와 함께 응답
      return res.status(401).json({ 
        success: false, 
        message: '아이디 혹은 비밀번호가 틀렸습니다.' 
      });
    }
  });
});

app.listen(port, () => {
  console.log(`서버가 포트 ${port}에서 실행 중입니다.`);
});
