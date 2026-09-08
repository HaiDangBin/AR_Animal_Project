# Hướng dẫn Git Branch cho nhóm 3 người — Đồ án AR Animal Project

## 1. Khái niệm cốt lõi

- `main` là **bản chính thức, luôn phải chạy được**. **Không ai code trực tiếp lên `main`.**
- Mỗi người tạo **branch riêng** để code, thử nghiệm, commit. Xong và chắc chắn ổn mới đưa vào `main`.
- Branch không tạo thêm thư mục — Git chỉ đổi nội dung file theo branch bạn đang "đứng".

## 2. Phân công vai trò (nhóm 3 người)

| Vai trò | Phạm vi công việc | Tên branch |
|---|---|---|
| **A — 3D & AR Core** | Model, Anchor, Collider, Plane Tracking, đặt vật thể AR | `feature/3d-anchor-setup` |
| **B — Data & Logic** | JSON localization, Raycast interaction, Audio system | `feature/json-audio-logic` |
| **C — UI/UX & Tích hợp** | TextMeshPro labels, Billboard script, Menu UI, testing | `feature/ui-labels` |

## 3. Phân công theo tuần

| Tuần | A (3D & AR) | B (Data & Logic) | C (UI/UX) |
|---|---|---|---|
| 1 | Tải & chọn model, setup `Models/` | Setup AR package (làm chung với A) | Setup `Resources/`, `Audio/` |
| 2 | **Chính:** Gắn Anchor + Collider, tạo Prefab | Research JSON schema | Tìm asset âm thanh |
| 3 | Hỗ trợ chỉnh Prefab nếu cần | **Chính:** Viết JSON đa ngôn ngữ, cài Newtonsoft.Json | Chuẩn bị UI wireframe |
| 4 | **Chính:** Plane Tracking, đặt vật thể AR | **Chính:** Script Raycast phát hiện chạm | **Chính:** TextMeshPro + FaceCamera.cs |
| 5 | Hỗ trợ test AR trên nhiều model | **Chính:** Kết nối Raycast → Audio → JSON | **Chính:** Menu chọn con vật, đổi ngôn ngữ |
| 6 | Tối ưu model (giảm poly, nén texture) | Debug logic, test đa điều kiện ánh sáng | Hoàn thiện báo cáo, demo/slide |

> ⚠️ **Tuần 4 dễ conflict nhất** vì cả 3 module liên quan nhau (Anchor → Raycast → Label). Họp ngắn đầu tuần 4: A hoàn thành Prefab + push trước, B và C mới build tiếp trên Prefab đó.

## 4. Quy trình làm việc hằng ngày (mỗi thành viên)

```bash
git checkout main
git pull                              # lấy code mới nhất của cả nhóm
git checkout -b feature/ten-viec      # lần đầu tạo branch, các lần sau: git checkout feature/ten-viec
# ... code trong Unity/VS Code ...
git add .
git commit -m "Mô tả ngắn gọn việc vừa làm"
git push -u origin feature/ten-viec
# → lên GitHub tạo Pull Request để merge vào main
```

## 5. Lệnh Git thường dùng

| Việc cần làm | Lệnh |
|---|---|
| Tạo branch mới | `git checkout -b feature/ten-viec` |
| Chuyển branch | `git checkout ten-branch` |
| Xem đang ở branch nào | `git branch` |
| Cập nhật main mới nhất | `git checkout main && git pull` |
| Đưa code lên branch của mình | `git push -u origin feature/ten-viec` |
| Đồng bộ main mới vào branch đang làm | `git checkout feature/ten-viec` rồi `git merge main` |

## 6. Quy trình Pull Request (PR)

1. Code xong trên branch riêng, push lên GitHub.
2. Vào repo → tab **Pull requests** → **New pull request**.
3. Chọn branch của mình → merge vào `main`.
4. **2 thành viên còn lại review** trước khi bấm **Merge**.
5. Sau khi merge, cả nhóm chạy `git checkout main && git pull` để lấy bản mới nhất.

## 7. Lưu ý đặc thù Unity — tránh hỏng Scene/Prefab

- **Tắt Unity trước khi `checkout` sang branch khác** — tránh Unity bị lag/lỗi vì file đổi đột ngột giữa chừng.
- File `.unity` (scene) và `.prefab` **rất khó merge tay** nếu 2 người cùng sửa — Git dễ báo conflict ngay cả khi thay đổi "trông không liên quan".
- **Bật Force Text Serialization**: `Edit → Project Settings → Editor → Asset Serialization → Force Text` (giúp file dễ đọc/merge hơn).
- **Không để 2 người cùng sửa 1 file Prefab/Scene cùng lúc** — trao đổi trong nhóm chat trước khi mở file chung.
- Luôn `git add` cả file `.meta` đi kèm — **đừng thêm `*.meta` vào `.gitignore`**, thiếu nó sẽ hỏng liên kết GUID (prefab/script mất reference).

## 8. Khi gặp Conflict

Terminal báo dạng:
```
CONFLICT (content): Merge conflict in Assets/Prefabs/Cat.prefab
```

**Cách xử lý:**
- Với file code `.cs`: Git thường tự merge được nếu 2 người sửa 2 hàm khác nhau.
- Với file `.unity`/`.prefab`: **không tự sửa bằng tay**. Nhắn nhóm, xác nhận ai đang giữ bản mới nhất, người còn lại bỏ thay đổi và làm lại phần đó dựa trên bản mới nhất.

## 9. Mẹo tránh conflict cho nhóm

- A tạo Prefab con vật xong, **commit + push trước** — B và C chỉ pull về dùng, không tự tạo bản Prefab riêng.
- Nếu B cần thêm component vào Prefab, báo nhóm trước khi mở file, tránh trùng thời điểm với C.
- Có thể tạo file `TEAM_LOG.md` trong repo, mỗi người ghi 1 dòng khi đang mở sửa file nào — cách "khóa" thủ công đơn giản, không cần Git LFS.

## 10. Các lỗi thường gặp và cách khắc phục

### Quên `.meta` khi add file mới
Mỗi file/folder trong Unity đều có file `.meta` đi kèm chứa GUID liên kết. Nếu quên add file này (thường do lọc nhầm trong `.gitignore`), người khác pull về sẽ bị mất liên kết prefab/script.

**Phòng tránh:** không thêm dòng `*.meta` vào `.gitignore`; luôn dùng `git add .` (dấu chấm) thay vì add từng file lẻ để không bỏ sót `.meta`.

### Conflict trong file `.unity`/`.prefab`
Đây là lỗi khó chịu nhất khi làm nhóm Unity — xem chi tiết cách xử lý ở Mục 8. Phòng tránh chủ động vẫn tốt hơn xử lý sau: tuân thủ Mục 9 (mẹo tránh conflict).

### Quên branch, code thẳng lên `main`
Nếu ai đó quên `git checkout -b ...` mà code luôn trên `main`, khi push sẽ đẩy thẳng vào bản chính — rất rủi ro.

**Cách phòng tránh triệt để:** Bật **Branch Protection Rule** cho `main` trên GitHub (Settings → Branches → Add rule → tick "Require a pull request before merging"). Sau khi bật, Git sẽ từ chối push thẳng vào `main`, bắt buộc phải tạo Pull Request.

**Cách kiểm tra đang ở branch nào trước khi code:**
```bash
git branch   # branch có dấu * là branch đang đứng
```

### Quên `git pull` trước khi bắt đầu code mới
Dễ dẫn đến code trên bản cũ, khi push lên bị lỗi do lệch lịch sử (`rejected - non-fast-forward`).

**Luôn bắt đầu buổi code bằng:**
```bash
git checkout main
git pull
git checkout ten-branch-cua-minh
git merge main   # đồng bộ code mới nhất vào branch đang làm
```

### Thành viên không merge được / không thấy nút Merge
Nếu đã bật Branch Protection yêu cầu review mà chưa có ai approve, nút Merge sẽ bị khóa. Đây là hoạt động **đúng như thiết kế**, không phải lỗi — cần có người trong nhóm vào tab Pull Requests và bấm **Approve** trước.

### Thành viên chưa clone được repo
Thường do chưa **Accept lời mời Collaborator** qua email/thông báo GitHub. Kiểm tra tại: `github.com/<tên-repo>/invitations`.
