using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using VimSts.Common;

namespace VimSts.GameScene
{
    [DefaultExecutionOrder(+1)]
    public class GameManager : MonoBehaviour
    {
        private const int GAME_STATE_READY    = 1;
        private const int GAME_STATE_GAMING   = 2;
        private const int GAME_STATE_FINISHED = 3;

        private const int KEY_STATE_WAIT_ANY             = 1;
        private const int KEY_STATE_WAIT_ANY_WITH_NUMBER = 2;
        private const int KEY_STATE_WAIT_ANY_WITH_G      = 3;
        private const int KEY_STATE_WAIT_SEARCH_CHAR     = 4;

        private const int KEY_SEARCH_TYPE_JUST_RIGHT     = 1;
        private const int KEY_SEARCH_TYPE_JUST_LEFT      = 2;
        private const int KEY_SEARCH_TYPE_AHEAD_OF_RIGHT = 3;
        private const int KEY_SEARCH_TYPE_AHEAD_OF_LEFT  = 4;

        private const int CHAR_TYPE_WORD     = 1;
        private const int CHAR_TYPE_BLANK    = 2;
        private const int CHAR_TYPE_NONBLANK = 3;

        public float sceneTransitionSec;
        public AudioClip gameBGM;
        public AudioClip nextSE;
        public AudioClip finishSE;
        public AudioClip selectSE;
        public GameObject restartLabel;
        public Text nTaskLinesText;
        public Text nKeyStrokeText;
        public Text totalTimeText;
        public TextMeshProUGUI taskLineText;
        public Animator fade;

        private SoundManager sound;
        private int gameState;
        private int nTaskLines;
        private int nFinishedTaskLines;
        private int nKeyStroke;
        private float totalTime;
        private string taskLine;
        private int cursorCol;
        private int exitCol;
        private int keyState;
        private int keyCount;
        private char keySearchChar;
        private int keySearchType;

        // lines from github.com/vim/vim/src/eval.c (with "sed '/^[ \t]*[\/\*]/d' | awk 'length > 60' | sed 's/^[ \t]*//' | shuf | head -100")
        private readonly string[] ALL_LINES = {
            "if (evaluate && in_vim9script() && !IS_WHITE_OR_NUL((*arg)[1]))",
            "temp_result = find_name_end(retval, &expr_start, &expr_end, 0);",
            "if (value_check_lock(ll_li->li_tv.v_lock, lp->ll_name, FALSE))",
            "else if (copyID != 0 && tv->vval.v_dict->dv_copyID == copyID",
            "if (list_concat(tv1->vval.v_list, tv2->vval.v_list, &var3) == FAIL)",
            "regmatch.regprog = vim_regcomp(pat, RE_MAGIC + RE_STRING);",
            "else if (eval1(arg, &var2, evalarg) == FAIL)  // recursive!",
            "abort = abort || set_ref_in_item(&wp->w_winvar.di_tv, copyID,",
            "ret = call_func_rettv(arg, evalarg, rettv, evaluate, NULL, &base);",
            "static int eval4(char_u **arg, typval_T *rettv, evalarg_T *evalarg);",
            "if (evaluate && vim9script && !IS_WHITE_OR_NUL((*arg)[1]))",
            "fill_evalarg_from_eap(evalarg_T *evalarg, exarg_T *eap, int skip)",
            "return echo_string_core(tv, tofree, numbuf, copyID, TRUE, FALSE, FALSE);",
            "if ((len == 1 && vim_strchr(NAMESPACE_CHAR, *arg) == NULL)",
            "int    use_sandbox = was_set_insecurely((char_u *)\"foldexpr\",",
            "static int eval2(char_u **arg, typval_T *rettv, evalarg_T *evalarg);",
            "&& (*p == NUL || (VIM_ISWHITE(p[-1]) && vim9_comment_start(p))))",
            "if (ri == NULL || (!lp->ll_empty2 && lp->ll_n2 == lp->ll_n1))",
            "else if (tv.v_type != VAR_STRING || tv.vval.v_string == NULL)",
            "garray_T    *gap = evalarg == NULL ? NULL : &evalarg->eval_ga;",
            "int      flags = evalarg == NULL ? 0 : evalarg->eval_flags;",
            "eval_next_non_blank(char_u *arg, evalarg_T *evalarg, int *getnext)",
            "ret = eval7_leader(rettv, TRUE, start_leader, &end_leader);",
            "vim_snprintf((char *)numbuf, NUMBUFLEN, \"%g\", tv->vval.v_float);",
            "abort = set_ref_in_ht(&dd->dv_hashtab, copyID, list_stack);",
            "if (eap != NULL && getline_equal(eap->getline, eap->cookie, getsourceline))",
            "xp->xp_context = has_expr ? EXPAND_EXPRESSION : EXPAND_NOTHING;",
            "if (evaluate && in_vim9script() && !IS_WHITE_OR_NUL((*arg)[2]))",
            "&& (evalarg->eval_cookie != NULL || evalarg->eval_cctx != NULL)",
            "echo_one(&rettv, eap->cmdidx == CMD_echo, &atstart, &needclr);",
            "else if (copyID != 0 && from->vval.v_dict->dv_copyID == copyID)",
            "pos.lnum = curwin->w_topline > 0 ? curwin->w_topline : 1;",
            "expr = skip_var_list(arg, TRUE, &fi->fi_varcount, &fi->fi_semicolon, FALSE);",
            "char_u  *p = echo_string(rettv, &tofree, numbuf, get_copyID());",
            "case '.':  ret = eval_number(arg, rettv, evaluate, want_string);",
            "lp->ll_exp_name = make_expanded_name(name, expr_start, expr_end, p);",
            "concat = op == '.' && (*(p + 1) == '.' || current_sctx.sc_version < 2);",
            "v = find_var(lp->ll_name, (flags & GLV_READ_ONLY) ? NULL : &ht,",
            "eval_expr_typval(typval_T *expr, typval_T *argv, int argc, typval_T *rettv)",
            "while (vim_regexec_nl(&regmatch, str, (colnr_T)(tail - str)))",
            "if (rettv->v_type == VAR_UNKNOWN && !evaluate && **arg == '(')",
            "if (evaluate && vim9script && !IS_WHITE_OR_NUL((*arg)[oplen]))",
            "int    save_flags = evalarg == NULL ? 0 : evalarg->eval_flags;",
            "list_join(&ga, tv.vval.v_list, (char_u *)\"\\n\", TRUE, FALSE, 0);",
            "if (eval1(&p, &var1, &EVALARG_EVALUATE) == FAIL)  // recursive!",
            "static int eval5(char_u **arg, typval_T *rettv, evalarg_T *evalarg);",
            "rettv->vval.v_number = result ? VVAL_TRUE : VVAL_FALSE;",
            "&& (evalarg->eval_cookie != NULL || evalarg->eval_cctx != NULL))",
            "sublen = vim_regsub(&regmatch, sub, expr, tail, FALSE, TRUE, FALSE);",
            "fill_evalarg_from_eap(&evalarg, eap, eap != NULL && eap->skip);",
            "for (li = cur_l->lv_first; !abort && li != NULL; li = li->li_next)",
            "return vim_strnsave(str + nbyte, MB_CPTR2LEN(str + nbyte));",
            "qsort(functions, (size_t)funcCnt, sizeof(struct fst), compare_func_name);",
            "set_ref_in_item(jq->jq_value, copyID, ht_stack, list_stack);",
            "case '/': n = num_divide(n, tv_get_number(tv2)); break;",
            "if (vim_strpbrk(arg, (char_u *)\"\\\"'+-*/%.=!?~|&$([<>,#\") == NULL)",
            "if (eval6(arg, &var2, evalarg, !vim9script && op == '.') == FAIL)",
            "&& tv2->v_type != VAR_BOOL && tv2->v_type != VAR_SPECIAL)",
            "abort = abort || set_ref_in_item(&aucmd_win->w_winvar.di_tv, copyID,",
            "static int eval6(char_u **arg, typval_T *rettv, evalarg_T *evalarg, int want_string);",
            "while ((!ends_excmd2(eap->cmd, arg) || *arg == '\"') && !got_int)",
            "for (cq = ch->ch_part[part].ch_cb_head.cq_next; cq != NULL;",
            "if (eap->cmdidx == CMD_echomsg || eap->cmdidx == CMD_echoerr)",
            "for (ri = rettv->vval.v_list->lv_first; ri != NULL && ll_li != NULL; )",
            "abort = abort || set_ref_in_item(&wp->w_winvar.di_tv, copyID,",
            "if (error || pos.lnum <= 0 || pos.lnum > curbuf->b_ml.ml_line_count)",
            "abort = set_ref_in_func(pt->pt_name, pt->pt_func, copyID);",
            "if ((op == '.' && tv_get_string_chk(rettv) == NULL) || error)",
            "if (vim9script && type_is && (p[len] == '?' || p[len] == '#'))",
            "for (jq = ch->ch_part[part].ch_json_head.jq_next; jq != NULL;",
            "static int eval7(char_u **arg, typval_T *rettv, evalarg_T *evalarg, int want_string);",
            "static int eval3(char_u **arg, typval_T *rettv, evalarg_T *evalarg);",
            "verb_msg(_(\"Not enough memory to set references, garbage collection aborted!\"));",
            "emsg(_(\"E698: variable nested too deep for making a copy\"));",
            "case '{':  ret = get_lambda_tv(arg, rettv, in_vim9script(), evalarg);",
            "if (call_func(s, -1, rettv, argc, argv, &funcexe) == FAIL)",
            "to->vval.v_list = list_copy(from->vval.v_list, deep, copyID);",
            "rettv->vval.v_number = result ? VVAL_TRUE : VVAL_FALSE;",
            "if (lp->ll_n2 - lp->ll_n1 + 1 != blob_len(rettv->vval.v_blob))",
            "&& ((rettv->v_type == VAR_DICT && *p == '.' && eval_isdictc(p[1]))",
            "set_var_const(lp->ll_name, lp->ll_type, rettv, copy, flags);",
            "while (*p == '[' || (*p == '.' && lp->ll_tv->v_type == VAR_DICT))",
            "if (value_check_lock(lp->ll_blob->bv_lock, lp->ll_name, FALSE))",
            "if (evaluate && in_vim9script() && !IS_WHITE_OR_NUL((*arg)[2]))",
            "evaluate = evalarg == NULL ? 0 : (evalarg->eval_flags & EVAL_EVALUATE);",
            "p = home_replace_save(NULL, get_scriptname(script_ctx.sc_sid));",
            "eap->getline, eap->cookie, DOCMD_NOWAIT|DOCMD_VERBOSE);",
            "mch_memmove((char_u *)ga.ga_data + ga.ga_len, tail, (size_t)i);",
            "abort = abort || set_ref_in_item(&HI2DI(hi)->di_tv, copyID,",
            "echo_one(typval_T *rettv, int with_space, int *atstart, int *needclr)",
            "if (expr_start != NULL && mb_nest == 0 && *expr_end == NULL)",
            "abort = abort || set_ref_in_item(&buf->b_bufvar.di_tv, copyID,",
            "set_ref_in_list_items(list_T *l, int copyID, ht_stack_T **ht_stack)",
            "if (!(lp->ll_tv->v_type == VAR_LIST && lp->ll_tv->vval.v_list != NULL)",
            "static char_u *make_expanded_name(char_u *in_start, char_u *expr_start, char_u *expr_end, char_u *in_end);",
            "|| (tv->vval.v_string != NULL && *tv->vval.v_string != NUL));",
            "if (rettv.v_type == VAR_CHANNEL || rettv.v_type == VAR_JOB)",
            "if (p[0] == 't' && p[1] == '_' && p[2] != NUL && p[3] != NUL)",
            "p = find_name_end(name, &expr_start, &expr_end, fne_flags);",
            "fill_evalarg_from_eap(&evalarg, eap, eap != NULL && eap->skip);",
        };

        void Start()
        {
            sound = SoundManager.FindInstance();
            sound.PlayBGM(gameBGM);

            fade.SetBool("faded", false);

            gameState = GAME_STATE_READY;
            StartGame();
        }

        void Update()
        {
            if (gameState == GAME_STATE_READY)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftBracket)))
                {
                    BackToMenu();
                }
            }
            else if (gameState == GAME_STATE_GAMING)
            {
                totalTime += Time.deltaTime;
                UpdateStatus();

                if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftBracket)))
                {
                    BackToMenu();
                }
                else if (Input.anyKeyDown)
                {
                    foreach (var key in Input.inputString)
                    {
                        AddKeyStroke(key, Input.GetKey(KeyCode.LeftControl));
                    }
                }
            }
            else if (gameState == GAME_STATE_FINISHED)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.LeftBracket)))
                {
                    BackToMenu();
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    RestartGame();
                }
            }
        }

        void UpdateStatus()
        {
            nTaskLinesText.text = $"{nFinishedTaskLines}/{nTaskLines}";
            nKeyStrokeText.text = $"{nKeyStroke}";
            totalTimeText.text  = $"{totalTime:f2}";
        }

        void UpdateTaskLine()
        {
            if (taskLine == "")
            {
                taskLineText.text = "";
                return;
            }

            var chars          = new List<char>(taskLine.ToCharArray());
            var decoratedChars = new List<string>();

            decoratedChars.Add("<mspace=14px>");
            for (var col = 0; col < chars.Count; col++)
            {
                var ch = chars[col];
                if (col == cursorCol-1)
                {
                    ch = ch == ' ' ? '_' : ch;
                    decoratedChars.Add($"<color=#009CCC>{ch}</color>");
                }
                else if (col == exitCol-1)
                {
                    decoratedChars.Add($"<color=#CE0A00>{ch}</color>");
                }
                else
                {
                    decoratedChars.Add($"{ch}");
                }
            }
            decoratedChars.Add("</mspace>");

            taskLineText.text = string.Join("", decoratedChars);
        }

        void AdjustCursorCol()
        {
            if (cursorCol <= 0)
            {
                cursorCol = 1;
            }
            else if (cursorCol > taskLine.Length)
            {
                cursorCol = taskLine.Length;
            }
        }

        void NextTaskLineWithUpdate()
        {
            if (nFinishedTaskLines != 0 && nFinishedTaskLines != nTaskLines)
            {
                sound.PlaySE(nextSE);
            }

            // 空白のみの行や1文字以下の行のときに無限ループすると思われるが、
            // 今のところは何も考えないことにする。
            taskLine = ALL_LINES[UnityEngine.Random.Range(0, ALL_LINES.Length)];
            AdjustCursorCol();

            exitCol = UnityEngine.Random.Range(0, taskLine.Length) + 1;
            while (exitCol == cursorCol || taskLine[exitCol-1] == ' ')
            {
                exitCol = UnityEngine.Random.Range(0, taskLine.Length) + 1;
            }

            UpdateTaskLine();
        }

        void CheckTaskLineWithUpdate()
        {
            if (cursorCol == exitCol)
            {
                nFinishedTaskLines++;
                UpdateStatus();

                if (nFinishedTaskLines == nTaskLines)
                {
                    FinishGame();
                }
                else
                {
                    NextTaskLineWithUpdate();
                }
            }

            UpdateTaskLine();
        }

        void AddKeyStroke(char newKey, bool withControl = false)
        {
            // Nh,<C-h>,<BS>,<Left> # 左にN桁移動
            // Nl,<Space>,<Right>   # 右にN桁移動
            // 0,<Home>             # 行の先頭に移動
            // ^                    # 行の先頭に移動(空白考慮)
            // $,<End>              # 行の末尾に移動
            // gM                   # 行の中央に移動
            // N|                   # N桁目に移動
            // Nf{char}             # 現在位置から右方向にあるN個目の{char}に移動
            // NF{char}             # 現在位置から左方向にあるN個目の{char}に移動
            // Nt{char}             # 現在位置から右方向にあるN個目の{char}の左側に移動
            // NT{char}             # 現在位置から左方向にあるN個目の{char}の右側に移動
            // N;                   # 直前の"f","F","t","T"をN回繰り返す
            // N,                   # 直前の"f","F","t","T"を逆方向にN回繰り返す
            // Nw                   # N個目の単語分、先に進む
            // NW                   # 空白で区切られた単語(=WORD)N個分、先に進む
            // Ne                   # N個目の単語のお尻まで進む
            // NE                   # 空白で区切られた単語(=WORD)N個目のお尻まで進む
            // Nb                   # N個目の単語分、前に戻る
            // NB                   # 空白で区切られた単語(=WORD)N個分、前に戻る
            // Nge                  # N個目の単語のお尻まで戻る
            // NgE                  # 空白で区切られた単語(=WORD)N個目のお尻まで戻る
            // %                    # 呼応するカッコ類("(","[","{")まで移動
            nKeyStroke++;

            if (keyState == KEY_STATE_WAIT_ANY)
            {
                if ('1' <= newKey && newKey <= '9')
                {
                    keyCount = newKey - '0';
                    keyState = KEY_STATE_WAIT_ANY_WITH_NUMBER;
                }
                else if (newKey == 'h' || newKey == '\b' || (newKey == 'h' && withControl))
                {
                    MoveCursorToLeft();
                }
                else if (newKey == 'l' || newKey == ' ')
                {
                    MoveCusrorToRight();
                }
                else if (newKey == '0')
                {
                    MoveCursorToBeginningOfLine();
                }
                else if (newKey == '^')
                {
                    MoveCursorToBeginningOfLineWithSkippedWhitespace();
                }
                else if (newKey == '$')
                {
                    MoveCursorToEndOfLine();
                }
                else if (newKey == 'g')
                {
                    keyState = KEY_STATE_WAIT_ANY_WITH_G;
                }
                else if (newKey == '|')
                {
                    MoveCursorToTheColumn();
                }
                else if (newKey == 'f')
                {
                    keySearchType = KEY_SEARCH_TYPE_JUST_RIGHT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == 'F')
                {
                    keySearchType = KEY_SEARCH_TYPE_JUST_LEFT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == 't')
                {
                    keySearchType = KEY_SEARCH_TYPE_AHEAD_OF_RIGHT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == 'T')
                {
                    keySearchType = KEY_SEARCH_TYPE_AHEAD_OF_LEFT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == ';')
                {
                    MoveCursorToNextSearch();
                }
                else if (newKey == ',')
                {
                    MoveCursorToPreviousSearch();
                }
                else if (newKey == 'w')
                {
                    MoveCursorToForeward();
                }
                else if (newKey == 'W')
                {
                    MoveCursorToBigForeward();
                }
                else if (newKey == 'e')
                {
                    MoveCursorToEndword();
                }
                else if (newKey == 'E')
                {
                    MoveCursorToBigEndword();
                }
                else if (newKey == 'b')
                {
                    MoveCursorToBackword();
                }
                else if (newKey == 'B')
                {
                    MoveCursorToBigBackword();
                }
                else
                {
                    ResetKeyStateToFirst();
                }
            }
            else if (keyState == KEY_STATE_WAIT_ANY_WITH_NUMBER)
            {
                if ('0' <= newKey && newKey <= '9')
                {
                    keyCount = keyCount*10 + (newKey-'0');
                }
                else if (newKey == 'h' || newKey == '\b' || (newKey == 'h' && withControl))
                {
                    MoveCursorToLeft();
                }
                else if (newKey == 'l' || newKey == ' ')
                {
                    MoveCusrorToRight();
                }
                else if (newKey == '^')
                {
                    MoveCursorToBeginningOfLineWithSkippedWhitespace();
                }
                else if (newKey == '$')
                {
                    MoveCursorToEndOfLine();
                }
                else if (newKey == 'g')
                {
                    keyState = KEY_STATE_WAIT_ANY_WITH_G;
                }
                else if (newKey == '|')
                {
                    MoveCursorToTheColumn();
                }
                else if (newKey == 'f')
                {
                    keySearchType = KEY_SEARCH_TYPE_JUST_RIGHT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == 'F')
                {
                    keySearchType = KEY_SEARCH_TYPE_JUST_LEFT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == 't')
                {
                    keySearchType = KEY_SEARCH_TYPE_AHEAD_OF_RIGHT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == 'T')
                {
                    keySearchType = KEY_SEARCH_TYPE_AHEAD_OF_LEFT;
                    keyState = KEY_STATE_WAIT_SEARCH_CHAR;
                }
                else if (newKey == ';')
                {
                    MoveCursorToNextSearch();
                }
                else if (newKey == ',')
                {
                    MoveCursorToPreviousSearch();
                }
                else if (newKey == 'w')
                {
                    MoveCursorToForeward();
                }
                else if (newKey == 'W')
                {
                    MoveCursorToBigForeward();
                }
                else if (newKey == 'e')
                {
                    MoveCursorToEndword();
                }
                else if (newKey == 'E')
                {
                    MoveCursorToBigEndword();
                }
                else if (newKey == 'b')
                {
                    MoveCursorToBackword();
                }
                else if (newKey == 'B')
                {
                    MoveCursorToBigBackword();
                }
                else
                {
                    ResetKeyStateToFirst();
                }
            }
            else if (keyState == KEY_STATE_WAIT_ANY_WITH_G)
            {
                 if (newKey == 'M')
                 {
                     MoveCursorToCenter();
                 }
                 else if (newKey == 'e')
                 {
                     MoveCursorToBackendword();
                 }
                 else if (newKey == 'E')
                 {
                     MoveCursorToBigBackendword();
                 }
                 else
                 {
                     ResetKeyStateToFirst();
                 }
            }
            else if (keyState == KEY_STATE_WAIT_SEARCH_CHAR)
            {
                keySearchChar = newKey;

                if (keySearchType == KEY_SEARCH_TYPE_JUST_RIGHT)
                {
                    MoveCursorToJustTheCharacterRight();
                }
                else if (keySearchType == KEY_SEARCH_TYPE_JUST_LEFT)
                {
                    MoveCursorToJustTheCharacterLeft();
                }
                else if (keySearchType == KEY_SEARCH_TYPE_AHEAD_OF_RIGHT)
                {
                    MoveCursorToAheadOfTheCharacterRight();
                }
                else if (keySearchType == KEY_SEARCH_TYPE_AHEAD_OF_RIGHT)
                {
                    MoveCursorToAheadOfTheCharacterLeft();
                }
                else
                {
                    ResetKeyStateToFirst();
                }
            }
            else
            {
                ResetKeyStateToFirst();
            }

            AdjustCursorCol();
            CheckTaskLineWithUpdate();
        }

        void ResetKeyStateToFirst()
        {
            keyState = KEY_STATE_WAIT_ANY;
            keyCount = 0;
        }

        void MoveCursorToLeft()
        {
            cursorCol -= Math.Max(1, keyCount);
            ResetKeyStateToFirst();
        }

        void MoveCusrorToRight()
        {
            cursorCol += Math.Max(1, keyCount);
            ResetKeyStateToFirst();
        }

        void MoveCursorToBeginningOfLine()
        {
            cursorCol = 1;
            ResetKeyStateToFirst();
        }

        void MoveCursorToBeginningOfLineWithSkippedWhitespace()
        {
            cursorCol = 1;
            for (var lineI = 0; lineI < taskLine.Length; lineI++)
            {
                var ch = taskLine[lineI];
                if (ch == ' ' || ch == '\t')
                {
                    cursorCol++;
                }
                else
                {
                    break;
                }
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToEndOfLine()
        {
            cursorCol = taskLine.Length;
            ResetKeyStateToFirst();
        }

        void MoveCursorToCenter()
        {
            cursorCol = (int)Math.Floor(taskLine.Length / 2.0) + 1;
            ResetKeyStateToFirst();
        }

        void MoveCursorToTheColumn()
        {
            cursorCol = Math.Max(1, keyCount);
            ResetKeyStateToFirst();
        }

        void MoveCursorToJustTheCharacterRight()
        {
            var nextI   = cursorCol - 1;
            var succeed = true;
            var nTries  = Math.Max(1, keyCount);
            for (var i = 0; i < nTries; i++)
            {
                if (nextI >= taskLine.Length-1)
                {
                    succeed = false;
                    break;
                }

                nextI++;
                nextI = taskLine.IndexOf(keySearchChar, nextI);
                if (nextI == -1)
                {
                    succeed = false;
                    break;
                }
            }
            if (succeed)
            {
                cursorCol = nextI + 1;
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToJustTheCharacterLeft()
        {
            var nextI   = cursorCol - 1;
            var succeed = true;
            var nTries  = Math.Max(1, keyCount);
            for (var i = 0; i < nTries; i++)
            {
                if (nextI <= 0)
                {
                    succeed = false;
                    break;
                }

                nextI--;
                nextI = taskLine.LastIndexOf(keySearchChar, nextI);
                if (nextI == -1)
                {
                    succeed = false;
                }
            }
            if (succeed)
            {
                cursorCol = nextI + 1;
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToAheadOfTheCharacterRight()
        {
            var nextI   = cursorCol - 1;
            var succeed = true;
            var nTries  = Math.Max(1, keyCount);
            for (var i = 0; i < nTries; i++)
            {
                if (nextI >= taskLine.Length-1)
                {
                    succeed = false;
                    break;
                }

                nextI++;
                nextI = taskLine.IndexOf(keySearchChar, nextI);
                if (nextI == -1)
                {
                    succeed = false;
                }
            }
            if (succeed)
            {
                cursorCol = nextI;
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToAheadOfTheCharacterLeft()
        {
            var nextI   = cursorCol - 1;
            var succeed = true;
            var nTries  = Math.Max(1, keyCount);
            for (var i = 0; i < nTries; i++)
            {
                if (nextI <= 0)
                {
                    succeed = false;
                    break;
                }

                nextI--;
                nextI = taskLine.LastIndexOf(keySearchChar, nextI);
                if (nextI == -1)
                {
                    succeed = false;
                }
            }
            if (succeed)
            {
                cursorCol = nextI;
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToNextSearch()
        {
            if (keySearchType == KEY_SEARCH_TYPE_JUST_RIGHT)
            {
                MoveCursorToJustTheCharacterRight();
            }
            else if (keySearchType == KEY_SEARCH_TYPE_JUST_LEFT)
            {
                MoveCursorToJustTheCharacterLeft();
            }
            else if (keySearchType == KEY_SEARCH_TYPE_AHEAD_OF_RIGHT)
            {
                MoveCursorToAheadOfTheCharacterRight();
            }
            else if (keySearchType == KEY_SEARCH_TYPE_AHEAD_OF_LEFT)
            {
                MoveCursorToAheadOfTheCharacterLeft();
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToPreviousSearch()
        {
            if (keySearchType == KEY_SEARCH_TYPE_JUST_RIGHT)
            {
                MoveCursorToJustTheCharacterLeft();
            }
            else if (keySearchType == KEY_SEARCH_TYPE_JUST_LEFT)
            {
                MoveCursorToJustTheCharacterRight();
            }
            else if (keySearchType == KEY_SEARCH_TYPE_AHEAD_OF_RIGHT)
            {
                MoveCursorToAheadOfTheCharacterLeft();
            }
            else if (keySearchType == KEY_SEARCH_TYPE_AHEAD_OF_LEFT)
            {
                MoveCursorToAheadOfTheCharacterRight();
            }

            ResetKeyStateToFirst();
        }

        void MoveCursorToForeward()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI+1 < taskLine.Length; i++)
            {
                var firstCharType = GetCharType(taskLine[curI]);
                var blankPassed   = firstCharType == CHAR_TYPE_BLANK;

                for (curI++; curI < taskLine.Length; curI++)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if ((blankPassed && charType != CHAR_TYPE_BLANK) || (charType != firstCharType && charType != CHAR_TYPE_BLANK))
                    {
                        break;
                    }

                    if (charType == CHAR_TYPE_BLANK)
                    {
                        blankPassed = true;
                    }
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToBigForeward()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI+1 < taskLine.Length; i++)
            {
                var firstCharType = GetCharType(taskLine[curI]);
                var blankPassed   = firstCharType == CHAR_TYPE_BLANK;

                for (curI++; curI < taskLine.Length; curI++)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (blankPassed && charType != CHAR_TYPE_BLANK)
                    {
                        break;
                    }

                    if (charType == CHAR_TYPE_BLANK)
                    {
                        blankPassed = true;
                    }
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToEndword()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI+1 < taskLine.Length; i++)
            {
                if (curI+2 == taskLine.Length)
                {
                    curI = taskLine.Length - 1;
                    break;
                }

                var charTypeA = GetCharType(taskLine[curI]);
                var charTypeB = GetCharType(taskLine[curI+1]);
                var startFromNextBlock = charTypeA != charTypeB;
                if (startFromNextBlock)
                {
                    curI++;
                }

                for (; curI < taskLine.Length; curI++)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType != CHAR_TYPE_BLANK)
                    {
                        break;
                    }
                }

                var firstCharType = GetCharType(taskLine[curI]);
                for (; curI < taskLine.Length; curI++)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType != firstCharType)
                    {
                        break;
                    }
                }
                if (curI < taskLine.Length)
                {
                    curI--;
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToBigEndword()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI+1 < taskLine.Length; i++)
            {
                if (curI+2 == taskLine.Length)
                {
                    curI = taskLine.Length - 1;
                    break;
                }

                var charTypeA = GetCharType(taskLine[curI]);
                var charTypeB = GetCharType(taskLine[curI+1]);
                var startFromNextBlock = (charTypeA != charTypeB) && charTypeB == CHAR_TYPE_BLANK;
                if (startFromNextBlock)
                {
                    curI++;
                }

                for (; curI < taskLine.Length; curI++)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType != CHAR_TYPE_BLANK)
                    {
                        break;
                    }
                }

                var firstCharType = GetCharType(taskLine[curI]);
                for (; curI < taskLine.Length; curI++)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType == CHAR_TYPE_BLANK)
                    {
                        break;
                    }
                }
                if (curI < taskLine.Length)
                {
                    curI--;
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToBackword()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI-1 >= 0; i++)
            {
                if (curI == 1)
                {
                    curI = 0;
                    break;
                }

                var charTypeA = GetCharType(taskLine[curI]);
                var charTypeB = GetCharType(taskLine[curI-1]);
                var startFromNextBlock = charTypeA != charTypeB;
                if (startFromNextBlock)
                {
                    curI--;
                }

                for (; curI >= 0; curI--)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType != CHAR_TYPE_BLANK)
                    {
                        break;
                    }
                }

                var firstCharType = GetCharType(taskLine[curI]);
                for (; curI >= 0; curI--)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType != firstCharType)
                    {
                        break;
                    }
                }
                if (curI >= 0)
                {
                    curI++;
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToBigBackword()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI-1 >= 0; i++)
            {
                if (curI == 1)
                {
                    curI = 0;
                    break;
                }

                var charTypeA = GetCharType(taskLine[curI]);
                var charTypeB = GetCharType(taskLine[curI-1]);
                var startFromNextBlock = (charTypeA != charTypeB) && charTypeB == CHAR_TYPE_BLANK;
                if (startFromNextBlock)
                {
                    curI--;
                }

                for (; curI >= 0; curI--)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType != CHAR_TYPE_BLANK)
                    {
                        break;
                    }
                }

                var firstCharType = GetCharType(taskLine[curI]);
                for (; curI >= 0; curI--)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (charType == CHAR_TYPE_BLANK)
                    {
                        break;
                    }
                }
                if (curI >= 0)
                {
                    curI++;
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToBackendword()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI-1 >= 0; i++)
            {
                var firstCharType = GetCharType(taskLine[curI]);
                var blankPassed   = firstCharType == CHAR_TYPE_BLANK;

                for (curI--; curI >= 0; curI--)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if ((blankPassed && charType != CHAR_TYPE_BLANK) || (charType != firstCharType && charType != CHAR_TYPE_BLANK))
                    {
                        break;
                    }

                    if (charType == CHAR_TYPE_BLANK)
                    {
                        blankPassed = true;
                    }
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        void MoveCursorToBigBackendword()
        {
            var curI  = cursorCol - 1;
            var nDoes = Math.Max(1, keyCount);
            for (var i = 0; i < nDoes && curI-1 >= 0; i++)
            {
                var firstCharType = GetCharType(taskLine[curI]);
                var blankPassed   = firstCharType == CHAR_TYPE_BLANK;

                for (curI--; curI >= 0; curI--)
                {
                    var charType = GetCharType(taskLine[curI]);
                    if (blankPassed && charType != CHAR_TYPE_BLANK)
                    {
                        break;
                    }

                    if (charType == CHAR_TYPE_BLANK)
                    {
                        blankPassed = true;
                    }
                }
            }
            cursorCol = curI + 1;

            ResetKeyStateToFirst();
        }

        int GetCharType(char ch)
        {
            if (('a' <= ch && ch <= 'z') || ('A' <= ch && ch <= 'Z') || ('0' <= ch && ch <= '9') || ch == '_')
            {
                return CHAR_TYPE_WORD;
            }
            else if (ch == ' ' || ch == '\t')
            {
                return CHAR_TYPE_BLANK;
            }
            else
            {
                return CHAR_TYPE_NONBLANK;
            }
        }

        void StartGame()
        {
            nTaskLines         = Setting.GetNTaskLines();
            nFinishedTaskLines = 0;
            nKeyStroke         = 0;
            totalTime          = 0f;
            taskLine           = "";
            cursorCol          = 1;
            exitCol            = 2;
            keyState           = KEY_STATE_WAIT_ANY;
            keyCount           = 0;
            keySearchChar      = '\n';
            UpdateStatus();
            NextTaskLineWithUpdate();

            gameState = GAME_STATE_GAMING;
        }

        void FinishGame()
        {
            gameState = GAME_STATE_FINISHED;

            sound.PlaySE(finishSE);
            restartLabel.SetActive(true);
        }

        void BackToMenu()
        {
            fade.SetBool("faded", true);
            StartCoroutine(WaitLoadScene("MenuScene"));
        }

        void RestartGame()
        {
            sound.PlaySE(selectSE);
            restartLabel.SetActive(false);

            gameState = GAME_STATE_READY;
            StartGame();
        }

        IEnumerator WaitLoadScene(string nextSceneName)
        {
            yield return new WaitForSeconds(sceneTransitionSec);
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
