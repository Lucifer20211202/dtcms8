<template>
	<div class="comment-box">
		<div class="comment-from">
			<div class="avatar-box">
				<el-image v-if="datas.userInfo.avatar" :src="common.loadFile(datas.userInfo.avatar)"></el-image>
				<span v-else class="icon iconfont icon-user-full"></span>
			</div>
			<div class="from-box">
				<div class="input">
					<el-input ref="commentRef" type="textarea" v-model="datas.commentForm.content"
						:placeholder="datas.commentForm.placeholder" maxlength="500" show-word-limit></el-input>
				</div>
				<div class="btn" @click="submitComment">发表</div>
			</div>
		</div>
		<div class="comment-list">
			<div v-if="datas.listData.length===0" class="nodata mart-20">暂无相关评论...</div>
			<div v-for="(item,index) in datas.listData" :key="index" class="list-box">
				<div class="avatar-box">
					<el-image v-if="item.userAvatar" :src="common.loadFile(item.userAvatar)"></el-image>
					<span v-else class="icon iconfont icon-user-full"></span>
				</div>
				<div class="content-box">
					<div class="head-box">
						<div class="meta">
							<span class="text">{{item.userName}}</span>
							<span class="time">{{item.dateDescription}}</span>
						</div>
						<div class="right">
							<span class="icon" @click="updateCommentLike(item)">
								<i class="iconfont icon-good"></i>
								{{item.likeCount}}
							</span>
							<span class="icon" @click="addCommentReply(item.id, item.userName)">
								<i class="iconfont icon-msg"></i>
								回复
							</span>
						</div>
					</div>
					<div class="content">
						{{item.content}}
					</div>
					<template v-if="item.children.length>0">
						<div v-for="(citem,cindex) in item.children" :key="cindex" class="reply">
							<span class="label">{{citem.userName}}@{{citem.atUserName}}</span>
							<span>{{citem.content}}</span>
							<div class="foot">
								<div class="left">
									<span>{{citem.dateDescription}}</span>
								</div>
								<div class="right">
									<span class="icon" @click="updateCommentLike(citem)">
										<i class="iconfont icon-good"></i>
										{{citem.likeCount}}
									</span>
									<span class="icon" @click="addCommentReply(citem.id, citem.userName)">
										<i class="iconfont icon-msg"></i>
										回复
									</span>
								</div>
							</div>
						</div>
					</template>
				</div>
				
			</div>
		</div>
	</div>
	
	<div class="pager-box" v-if="datas.totalCount>0">
		<el-pagination background layout="prev, pager, next"
			:page-size="props.pageSize"
			:current-page="datas.pageIndex"
			:total="datas.totalCount"
			@current-change="handleCurrentChange" />
	</div>
</template>

<script setup>
	//获取当前站点信息
	const siteConfig = await useSite('site')
	//声明Ref
	const commentRef = ref(null)
	//接收props传值
	const props = defineProps({
		//文章ID
		articleId: {
			type: Number,
			default: () => {
				return 0
			}
		},
		//每页数量
		pageSize: {
			type: Number,
			default: () => {
				return 10
			}
		},
	})
	//声明变量
	const datas = reactive({
		loading: true,
		totalCount: 0, //总数量
		pageIndex: 1, //当前页码
		listData: [], //评论列表
		userInfo: {}, //用户信息
		commentForm: {
			parentId: 0,
			placeholder: '我来说几句',
			content: '',
		}
	})
	
	//初始化数据
	const initData = async() => {
		await useHttp({
			url: "/client/member",
			success(res) {
				datas.userInfo = res.data
			},
		}).catch((error) => {
			//为了让程序继续进行，在这里处理错误
		})
		//加载评论
		loadData()
	}
	//加载评论列表
	const loadData = async() => {
		await useHttp({
			url: `/client/article/comment/${props.articleId}?pageSize=${props.pageSize}&pageIndex=${datas.pageIndex}`,
			success(res) {
				datas.listData = res.data
				datas.totalCount = res.pagination.totalCount
				datas.pageIndex = res.pagination.pageIndex
			},
			error(err) {
				datas.listData = []
			},
			complete() {
				datas.loading = false
			},
		})
	}
	//跳转到第几页
	const handleCurrentChange = (val) => {
		if (datas.pageIndex != val) {
			datas.pageIndex = val
			loadData()
		}
	}
	//提交评论
	const submitComment = () => {
		//如果没有填写内容
		if (!datas.commentForm.content) {
			ElMessage({ type: 'warning', message: '请填写评论内容' })
			return false
		}
		//拼接参数
		datas.commentForm.siteId = siteConfig.id
		datas.commentForm.articleId = props.articleId
		//正式提交
		useHttp({
			url: `/account/article/comment/add`,
			method: "post",
			data: datas.commentForm,
			successMsg: '评论已发表成功',
			success(res) {
				//追加评论信息
				if (res.data.parentId == 0) {
					//一级评论
					datas.listData.push(res.data)
				} else {
					//回复评论
					datas.listData.forEach((item,index) => {
						if (item.id == res.data.rootId) {
							item.children.push(res.data)
						}
					})
				}
				//恢复初始评论表单
				datas.commentForm.parentId = 0
				datas.commentForm.placeholder = '我来说几句'
				datas.commentForm.content = ''
			}
		})
	}
	//评论点赞
	const updateCommentLike = (obj) => {
		useHttp({
			url: `/account/article/comment/like/${obj.id}`,
			method: "put",
			success(res) {
				obj.likeCount = res.data
			}
		})
	}
	//点击回复
	const addCommentReply = (commentId, userName) => {
		datas.commentForm.parentId = commentId
		datas.commentForm.placeholder = `回复 ${userName}`
		commentRef.value.focus()
	}
	
	//页面完成后执行
	onMounted(() => {
		initData()
	})
</script>

<style lang="scss">
	.comment-box {
		margin-top: 1.25rem;
		.comment-from {
			display: flex;
			justify-content: flex-start;
			margin-bottom: 1.25rem;
			.avatar-box {
				display: flex;
				justify-content: center;
				align-items: center;
				background: #fff;
				box-shadow: 0px 6px 35px rgba(0,0,0,0.02);
				width: 50px;
				height: 50px;
				border-radius: 50%;
				overflow: hidden;
				>img,.el-image {
					width: 100%;
					height: 100%;
				}
				.icon {
					color: var(--muted-3-color);
					font-size: 1.5rem;
				}
			}
			.from-box {
				flex-grow: 1;
				display: flex;
				position: relative;
				margin-left: 1.25rem;
				height: 6rem;
				.input {
					position: relative;
					display: block;
					padding: 0.5rem;
					width: 100%;
					height: 100%;
					font-size: 1rem;
					line-height: 1.5em;
					color: #4b4b4b;
					border: 1px solid #f0f0f0;
					border-right: none;
					border-top-left-radius: var(--main-radius);
					border-bottom-left-radius: var(--main-radius);
					box-shadow: 0 0 10px var(--main-shadow);
					background: #fff;
					.el-textarea {
						height: 100%;
						textarea {
							font-size: 1rem;
							border: none;
							box-shadow: none;
							height: 100%;
						}
					}
				}
				.btn {
					width: 100px;
					height: 6rem;
					line-height: 6rem;
					color: #fff;
					font-weight: 600;
					text-align: center;
					background: #379be9;
					border-top-right-radius: var(--main-radius);
					border-bottom-right-radius: var(--main-radius);
					box-shadow: 0 0 10px var(--main-shadow);
					cursor: pointer;
					&:hover{
						background: #328bd1;
					}
				}
			}
		}
		.comment-list {
			.list-box {
				display: flex;
				justify-content: space-between;
				margin-top: 1.25rem;
				padding: 1.25rem;
				background: var(--main-bg-color);
				box-shadow: 0 0 10px var(--main-shadow);
				border-radius: var(--main-radius);
				.avatar-box {
					flex-shrink: 0;
					display: flex;
					justify-content: center;
					align-items: center;
					background: #f7f8fa;
					box-shadow: 0px 6px 35px rgba(0,0,0,0.02);
					margin-right: 1.25rem;
					width: 50px;
					height: 50px;
					border-radius: 50%;
					overflow: hidden;
					img,.el-image {
						width: 100%;
						height: 100%;
					}
					.icon {
						color: var(--muted-3-color);
						font-size: 1.5rem;
					}
				}
				.content-box {
					flex-grow: 1;
					.head-box {
						display: flex;
						justify-content: space-between;
						.meta {
							display: flex;
							flex-direction: column;
							.text {
								color: var(--muted-color);
								font-size: 1rem;
								line-height: 1em;
							}
							.time {
								margin-top: 1rem;
								color: var(--muted-2-color);
								font-size: 0.75rem;
								line-height: 1em;
							}
						}
						.right {
							.icon {
								margin-left: 10px;
								color: var(--muted-2-color);
								font-size: 0.875rem;
								cursor: pointer;
								i {
									margin-right: 1px;
									font-size: 0.875rem;
								}
							}
						}
					}
					.content {
						margin-top: 1rem;
						color: var(--muted-color);
						font-size: 1rem;
						line-height: 1.5em;
					}
					.reply {
						margin-top: 1rem;
						padding: 1.25rem;
						color: var(--muted-color);
						font-size: 1rem;
						line-height: 1.5em;
						background: #f7f8fa;
						border-radius: var(--main-radius);
						overflow: hidden;
						.label {
							margin-right: 10px;
							color: var(--muted-3-color);
						}
						.foot {
							display: flex;
							justify-content: space-between;
							align-items: center;
							margin-top: 1rem;
							.left {
								color: var(--muted-2-color);
								font-size: 0.75rem;
							}
							.right {
								.icon {
									margin-left: 10px;
									color: var(--muted-2-color);
									font-size: 0.875rem;
									cursor: pointer;
									i {
										margin-right: 2px;
										font-size: 0.875rem;
									}
								}
							}
						}
					}
				}
			}
		}
	}
</style>