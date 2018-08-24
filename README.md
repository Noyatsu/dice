# dice
さいころコロコロゲーム(Unity使用予定)

# contributing
このプロジェクトに参加する前に以下を必ず読むこと。

## commit messageの注意
このフォーマットに従うこと: `"[commit type] コミットの概要"`

commit typeは以下のものを使用すること:
* **fix**: バグ修正
* **hotfix**: 重大なバグ修正
* **add**: 関数やファイルの追加
* **update**: バグ修正ではない関数の変更
* **clean**: 整理 (リファクタリングなど)
* **change**: 仕様の変更
* **disable**: 無効化 (コメントアウトなど)
* **remove**: 関数やファイルの削除
* **upgrade**: バージョンアップ
* **revert**: 事前のコミットをキャンセル

## Issue / Pull Requestの注意
### Issueのタグ使い分け
Issueには必ず以下のいずれかを付与し、状況が変化した場合はそれに合わせて変更すること。
* **standy by**: 待機中。コンフリクトの危険がある、仕様が固まっていない等、まだこのIssueの対応をしてほしくないとき。
* **to do**: 対応待ち。
* **doing**: 現在誰かが対応しているとき。

また、状況に応じて以下のタグを付与する。
* **question**: 他の人の意見を聞きたいとき。
* **hot fix**: 修正が緊急を要する時。
* **bug**: バグ報告の際。

また、Issueをcloseする際、必要に応じて以下のタグを付与する。
* **duplicate**: 他のIssueと重複した場合。
* **wontfix**: 修正の必要が無い場合。
のタグ使い分け

### Pull Requestの注意
Pull Requestはコンフリクトしないようならセルフマージしてよい。[要出典]

将来的にはrebaseも検討。

